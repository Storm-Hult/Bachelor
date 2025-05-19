# B025EB-47 Sjakkrobot

## Project Overview
This project is a fully autonomous chess-playing robot designed to recognize a chessboard, identify piece positions, calculate moves using a chess engine, and physically execute those moves using a robotic arm. The system is capable of playing against a human opponent and functions without human intervention once a game begins.

## Authors
- Storm Hultgren
- Elias Trones Jåstad

## Supervisor
- Frikk Hosøy Fossdal

## System Components
- **Camera System**: Recognizes the chessboard layout, detects piece positions, and tracks changes.
- **Convolutional Neural Network (CNN)**: Identifies each chess piece using image classification.
- **Chess Engine**: Uses Stockfish to calculate legal and strategic moves.
- **Robotic Arm (UR5e)**: Executes moves calculated by the chess engine.
- **Graphical User Interface (GUI)**: Developed using C# in Visual Studio, offering two main modes:
  - Bot Play: Play against the robot using Stockfish.
  - Online Play (planned): Play against human opponents online.

## Key Features
- Automatic piece recognition using a CNN.
- Accurate board recognition using ArUco markers in OpenCV.
- Stockfish integration for move calculation.
- Robotic arm for precise piece manipulation.
- Error detection and recovery system.
- Move execution in under 30 seconds (in most cases).
- User-friendly C# GUI with multiple play modes.
