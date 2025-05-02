import socket
import time
import struct
import sys
import json

ROBOT_IP = '169.254.194.28'
DASHBOARD_PORT = 29999
RTDE_PORT = 30002

def send_cmd(sock, cmd):
    sock.sendall(cmd.encode() + b'\n')
    return sock.recv(1024).decode().strip()

def grip_open(sock):
    send_cmd(sock, 'load grip_open.urp')
    send_cmd(sock, 'play')
    time.sleep(1.5)

def grip_close(sock):
    send_cmd(sock, 'load grip_close.urp')
    send_cmd(sock, 'play')
    time.sleep(1.5)

def wait_until_position_reached(sock, target_pose, threshold=0.01):
    import struct
    def read_packet(sock):
        header = sock.recv(4)
        if len(header) < 4:
            return None
        packet_size = struct.unpack('!i', header)[0]
        payload = b''
        while len(payload) < packet_size - 4:
            chunk = sock.recv(packet_size - 4 - len(payload))
            if not chunk:
                break
            payload += chunk
        return header + payload if len(payload) == packet_size - 4 else None

    while True:
        packet = read_packet(sock)
        if packet is None:
            continue
        actual_pose = struct.unpack('!6d', packet[252:300])
        time.sleep(0.1)
        if all(abs(a - b) < threshold for a, b in zip(actual_pose, target_pose)):
            break

def move_p(pose, acc=0.8, vel=0.3):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as rtde_sock:
        rtde_sock.connect((ROBOT_IP, RTDE_PORT))
        cmd = f"movep(p{pose}, a={acc}, v={vel})\n"
        print(f"Sending command (movep): {cmd.strip()}")
        rtde_sock.sendall(cmd.encode())
    wait_until_position_reached(pose)

def move_l(pose, acc=0.8, vel=0.2):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as rtde_sock:
        rtde_sock.connect((ROBOT_IP, RTDE_PORT))
        cmd = f"movel(p{pose}, a={acc}, v={vel})\n"
        print(f"Sending command (movel): {cmd.strip()}")
        rtde_sock.sendall(cmd.encode())
    wait_until_position_reached(pose)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python script.py '<waypoints_json>'")
        sys.exit(1)

    waypoints_json = sys.argv[1]
    try:
        waypoints = json.loads(waypoints_json)
    except Exception as e:
        print("Error parsing JSON:", e)
        sys.exit(1)

    dashboard_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    dashboard_sock.connect((ROBOT_IP, DASHBOARD_PORT))
    lower_count = 0
    grip_open(dashboard_sock)
    #lower_count += 1
    print("Dashboard response:", dashboard_sock.recv(1024).decode().strip())

    offBoardPose = [0, -0.7, 0.19, 0, 3.14, 0]
    prev_pose = None

    for curr_pose in waypoints:
        print("Moving to waypoint:", curr_pose)

        if prev_pose is not None:
            same_xy = abs(prev_pose[0] - curr_pose[0]) < 5e-3 and abs(prev_pose[1] - curr_pose[1]) < 5e-3
            down_from_new_xy = abs(curr_pose[2] - 0.155) < 3e-3 and (abs(prev_pose[0] - curr_pose[0]) > 0.02 or abs(prev_pose[1] - curr_pose[1]) > 0.02)
            if same_xy or down_from_new_xy:
                move_l(curr_pose)
            else:
                move_p(curr_pose)
        else:
            move_p(curr_pose)

        if all(abs(curr_pose[i] - offBoardPose[i]) < 2e-2 for i in range(6)):
            print("Reached off-board position. Releasing piece.")
            grip_open(dashboard_sock)
            lower_count += 1
        elif abs(curr_pose[2] - 0.055) < 3e-3:
            lower_count += 1
            if lower_count % 2 == 1:
                print("Lowered pose for pickup. Closing gripper.")
                grip_close(dashboard_sock)
            else:
                print("Lowered pose for placement. Opening gripper.")
                grip_open(dashboard_sock)

        prev_pose = curr_pose

    dashboard_sock.close()
