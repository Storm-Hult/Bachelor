import os
import glob
import cv2
import numpy as np
import torch
import torch.nn as nn
from torchvision import models, transforms
from cv2 import aruco
from torchvision.models import efficientnet_b0

# -------- Konstanter ---------
input_file = r"C:\Users\Elias\Camera_folder\image_test.png"
output_folder = r"C:\Users\Elias\Camera_folder\Recent_board"
model_path = r"C:\Users\Elias\Documents\skole\best_model_gray130.pth"
efficientnet_path = r"C:\Users\Elias\Documents\skole\efficientnet_b0_gray130.pth"

# -------- Slett eksisterende bilder ---------
if os.path.exists(output_folder):
    files = glob.glob(os.path.join(output_folder, "*.png"))
    for f in files:
        os.remove(f)
else:
    os.makedirs(output_folder)

# -------- Last inn EfficientNet-B0 (gråskala + 13 klasser) ---------
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

model = models.efficientnet_b0(weights=None)

# Endre input-lag til 1-kanal
original_conv = model.features[0][0]
model.features[0][0] = nn.Conv2d(
    in_channels=1,
    out_channels=original_conv.out_channels,
    kernel_size=original_conv.kernel_size,
    stride=original_conv.stride,
    padding=original_conv.padding,
    bias=original_conv.bias is not None
)

# Endre klassifikasjonslaget til 13 utganger
model.classifier = nn.Sequential(
    nn.Dropout(0.4),
    nn.Linear(1280, 13)
)

# Last inn base + trenede vekter
model.load_state_dict(torch.load(efficientnet_path, map_location=device))
model.load_state_dict(torch.load(model_path, map_location=device))
model.to(device)
model.eval()

# -------- Les originalbildet ---------
image = cv2.imread(input_file)
if image is None:
    print("Kunne ikke lese inputbildet:", input_file)
    exit()

gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# -------- ArUco-deteksjon ---------
aruco_dict = aruco.getPredefinedDictionary(aruco.DICT_4X4_250)
parameters = aruco.DetectorParameters()
detector = aruco.ArucoDetector(aruco_dict, parameters)
corners, ids, _ = detector.detectMarkers(gray)

if ids is not None:
    ref_map = {13: 2, 42: 2, 21: 2, 36: 1}
    pts = {}
    for i, marker_id in enumerate(ids.flatten()):
        if marker_id in ref_map:
            pts[marker_id] = corners[i][0][ref_map[marker_id]]
    
    if all(key in pts for key in [13, 42, 21, 36]):
        src_pts = np.array([pts[13], pts[42], pts[36], pts[21]], dtype="float32")

        width_top = np.linalg.norm(src_pts[1] - src_pts[0])
        width_bottom = np.linalg.norm(src_pts[2] - src_pts[3])
        maxWidth = int(max(width_top, width_bottom))

        height_left = np.linalg.norm(src_pts[3] - src_pts[0])
        height_right = np.linalg.norm(src_pts[2] - src_pts[1])
        maxHeight = int(max(height_left, height_right))

        dst_pts = np.array([
            [0, 0], [maxWidth - 1, 0],
            [maxWidth - 1, maxHeight - 1], [0, maxHeight - 1]
        ], dtype="float32")

        M = cv2.getPerspectiveTransform(src_pts, dst_pts)
        warped = cv2.warpPerspective(image, M, (maxWidth, maxHeight))

        margin_percent = 0.005
        h_warp, w_warp = warped.shape[:2]
        margin_x = int(w_warp * margin_percent)
        margin_y = int(h_warp * margin_percent)
        cropped = warped[margin_y:h_warp - margin_y, margin_x:w_warp - margin_x]

        h_crop, w_crop = cropped.shape[:2]
        cell_h = h_crop // 8
        cell_w = w_crop // 8

        cell_filenames = []
        for row in range(8):
            for col in range(8):
                cell = cropped[row * cell_h:(row + 1) * cell_h, col * cell_w:(col + 1) * cell_w]
                cell_filename = os.path.join(output_folder, f"cell_{row}{col}.png")
                cv2.imwrite(cell_filename, cell)
                cell_filenames.append((cell_filename, row, col))

        # -------- Klassifiser hver celle ---------
        transform = transforms.Compose([
            transforms.ToPILImage(),
            transforms.Grayscale(num_output_channels=1),
            transforms.Resize((130, 130)),
            transforms.ToTensor(),
            transforms.Normalize([0.5], [0.5])
        ])

        board_string = [""] * 64

        piece_map = {
            0: "b",  1: "k", 2: "n", 3: "p", 4: "q", 5: "r",
            6: ".", 7: "B", 8: "K", 9: "N", 10: "P", 11: "Q", 12: "R"
        }

        for filename, row, col in cell_filenames:
            img = cv2.imread(filename)
            if img is None:
                print(f"Kunne ikke lese bilde: {filename}")
                continue
            img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
            img = transform(img).unsqueeze(0).to(device)

            with torch.no_grad():
                prediction = model(img)
                predicted_class = torch.argmax(prediction).item()

            index = row * 8 + col
            board_string[index] = piece_map.get(predicted_class, "?")

        final_board_string = "".join(board_string)
        print(final_board_string)

    else:
        print("Ikke alle nødvendige ArUco-markører ble funnet!")
else:
    print("Ingen ArUco-markører funnet!")