import socket
import sys

ROBOT_IP = '169.254.194.28'
RTDE_PORT = 30003
DASHBOARD_PORT = 29999

def send_cmd(sock, cmd):
    sock.sendall(cmd.encode() + b'\n')
    return sock.recv(1024).decode().strip()

def move_p(pose, acc=0.8, vel=0.3):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as rtde_sock:
        rtde_sock.connect((ROBOT_IP, RTDE_PORT))
        pose_str = ', '.join(f"{x:.6f}" for x in pose)  # eksakt og ryddig
        cmd = f"movep(p[{pose_str}], a={acc}, v={vel})\n"
        print(f"Sending command (movep): {cmd.strip()}")
        rtde_sock.sendall(cmd.encode())

def grip_open(sock):
    send_cmd(sock, 'load grip_open.urp')
    send_cmd(sock, 'play')
    time.sleep(1.5)

def grip_close(sock):
    send_cmd(sock, 'load grip_close.urp')
    send_cmd(sock, 'play')
    time.sleep(1.5)

if __name__ == "__main__":
    StartPose = [0, -0.534, 0.5, 0, 3.14, 0]
    dashboard_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    dashboard_sock.connect((ROBOT_IP, DASHBOARD_PORT))
    #grip_close(dashboard_sock)
    grip_open(dashboard_sock)
    move_p(StartPose)