# 🧩 Sudology

**Unity 2D Sudoku 퍼즐 게임 (7일 완성, 1인 개발, 포트폴리오용)**  
- 웹(WebGL) 완벽 지원  
- SOLID 원칙, 싱글톤, 캡슐화, 구조적 리팩터링 적용

---

## 🌐 [웹에서 바로 플레이 (Netlify)](https://sudology-demo.netlify.app/)

> WebGL 데모는 크롬, 엣지, 파이어폭스 등 최신 브라우저에서 플레이 가능

---

## 🎮 주요 기능

- **9x9 스도쿠 퍼즐 자동 생성 (유일 해답 보장)**
- **난이도 선택:** Easy / Normal / Hard
- **숫자 입력 & 셀 하이라이트**
- **채점(오답 표시)**
- **힌트(광고 없음, 퍼즐당 3회)**
- **Undo / Redo (횟수 제한 없음)**
- **통계:** 난이도별 클리어 횟수, 최고/평균 클리어 타임, 평균 힌트 사용
- **배너 광고:** Android (Unity Ads, WebGL에서는 미출력)
- **저장/불러오기, 입력 상태 유지, 통계 JSON 저장**
- **SOLID 원칙, 싱글톤, 주석, UI/매니저 구조 최적화

---

## 📁 프로젝트 구조

- Assets/
- ├── Scenes/
- │   ├── StartScene.unity        # 메인 메뉴, 난이도 선택, Continue, Settings, Stats
- │   └── GameScene.unity         # 퍼즐 플레이, Back/Settings/Stats/배너 광고
- │
- ├── Scripts/
- │   ├── Ads/
- │   │   ├── AdsInitializer.cs   # 유니티 광고 시스템 초기화
- │   │   └── BannerAd.cs         # 배너 광고 로딩/표시/숨김 제어
- │   │
- │   ├── Audio/
- │   │   └── AudioManager.cs     # 효과음/SFX/BGM 재생/볼륨 관리
- │   │
- │   ├── Core/
- │   │   ├── Difficulty.cs       # 난이도 Enum/설정값
- │   │   ├── GameManager.cs      # 게임 흐름/씬 전환/퍼즐 상태(새 게임/이어하기) 총괄
- │   │   ├── SaveManager.cs      # 퍼즐/진행 상태 JSON 저장·불러오기
- │   │   └── SceneLoader.cs      # 씬 전환(Back/Continue) 기능
- │   │
- │   ├── Data/
- │   │   ├── StatsData.cs        # 통계 데이터 구조체/클래스
- │   │   └── Wrapper.cs          # 제네릭 JSON 래퍼/직렬화 도우미
- │   │
- │   ├── Puzzle/
- │   │   ├── IPuzzleGenerator.cs # 퍼즐 생성 알고리즘 인터페이스(확장성)
- │   │   ├── PuzzleCell.cs       # 1칸 셀(값, 상태, 정답, 입력/고정)
- │   │   ├── PuzzleGenerator.cs  # 퍼즐/정답 보장 생성기(알고리즘)
- │   │   └── PuzzleValidator.cs  # 퍼즐 정답/입력 유효성 검증
- │   │
- │   ├── UI/
- │   │   ├── DifficultySelector.cs # 난이도 선택 드롭다운/버튼 제어
- │   │   ├── GameClearPanel.cs     # 클리어 시 UI/통계 표시
- │   │   ├── GameTimer.cs          # 게임 타이머/시간 측정
- │   │   ├── HintManager.cs        # 힌트 기능/횟수 관리
- │   │   ├── InputManager.cs       # 셀 선택/숫자입력/Undo/Redo
- │   │   ├── PuzzleManager.cs      # 퍼즐판 생성/초기화/입력 연결
- │   │   ├── SettingsManager.cs    # SettingsPanel(다크모드, 사운드 등)
- │   │   ├── StartUIManager.cs     # StartScene UI 버튼/패널
- │   │   └── StatsManager.cs       # 난이도별 통계 저장/불러오기/통계 UI
- │   │
- │   └── Util/
- │       (현재 없음/유틸 추가 가능)
- │
- ├── Prefabs/
- │   └── PuzzleCell.prefab        # 1칸 셀 프리팹 (cellText, isFixed, correctValue 등)
- │
- ├── Resources/
- │   └── (사운드, 기타 리소스)
- │
- ├── Sounds/
- │   └── (게임 효과음/BGM)
- │
- └── ... (기타 에셋/플러그인 등)

---

## 📅 개발 일정 및 진행 현황

| Day       | 주요 작업                                                                                                                     |
|-----------|-------------------------------------------------------------------------------------------------------------------------------|
| **Day 1** | 프로젝트 초기 설정, UI 프리팹 구성, GitHub 커밋                                                                                  |
| **Day 2** | 퍼즐 생성기 구현, 문제 셀 잠금, SOLID 구조 적용, 난이도 선택 연동                                                                  |
| **Day 3** | 숫자 입력 & 셀 선택, 채점 기능, 힌트, Undo/Redo, 입력 상태 저장·복원                                                              |
| **Day 4** | StartScene UI에 난이도 선택 패널 연동 및 UI 제어, 저장 초기화·Continue 버튼 로직 추가, GameManager 난이도 설정 후 퍼즐 생성 연결     |
| **Day 5** | Back/Continue 연동, SettingsPanel·StatsPanel UI 뼈대, Unity Ads 배너 연동 및 Canvas 설정                                        |
| **Day 6** | 퍼즐 저장/불러오기 구조 일원화, 핵심 기능 완성, 동작 보장, 모든 버그 및 기능 작동 확인                                               |
| **Day 7** | 코드 최적화, 구조 분리, 주석 보강, WebGL 빌드, Netlify 배포, README 문서화                                                        |


