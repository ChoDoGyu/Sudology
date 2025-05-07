# 🧩 Sudology

**7일 만에 1인 개발로 만든 Unity 2D 스도쿠 퍼즐 게임**입니다.  
Android 플랫폼 기준으로 설계되었으며, GitHub 포트폴리오용으로 제작되었습니다.

---

## 🎮 주요 기능

- 9x9 스도쿠 퍼즐 생성기
- 난이도 선택 (쉬움 / 보통 / 어려움)
- 정답 채점 기능 (오답 표시)
- 힌트 기능 (광고 없이, 퍼즐당 3회 제한)
- Undo / Redo 기능 (최대 3회)
- 입력 기록 저장 및 복원 (앱 종료 후 재시작)
- 난이도별 통계 저장
- 배너 광고 (Unity Ads)

---

## 🧱 개발 환경

- Unity 2022.3.42f1 LTS
- C#
- Visual Studio 2022
- GitHub Desktop
- Android 플랫폼 (Portrait UI)

---

## 📁 프로젝트 구조

- Assets/
  - Scripts/
    - Difficulty.cs
    - DifficultySelector.cs
    - GameManager.cs
    - IPuzzleGenerator.cs
    - PuzzleGenerator.cs
    - PuzzleCell.cs
    - UIManager.cs
    - InputManager.cs
    - AudioManager.cs
  - Prefabs/
    - PuzzleCell.prefab
  - Resources/
  - Scenes/
    - StartScene.unity
    - GameScene.unity
  - Audio/

---

## 💡 기획 배경

- Unity 2D 기초를 연습하고
- 기능 중심의 퍼즐 게임 구조를 학습하며
- Google Play Store 출시까지 진행 가능한 최소 기능(MVP) 게임을 구현하는 것이 목표였습니다.

- ## 📷 스크린샷

> (게임 실행 화면이 있다면 여기에 이미지 추가)

---
