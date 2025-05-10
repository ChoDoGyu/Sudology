# 🧩 Sudology

**7일 만에 1인 개발로 만든 Unity 2D 스도쿠 퍼즐 게임**  
Android/Portrait 모드용으로 설계되었으며, GitHub 포트트폴리오용으로 제작되었습니다.

---

## 🎮 주요 기능

- 9×9 스도쿠 퍼즐 자동 생성 (백트래킹 + 유일 해답 보장)  
- 난이도 선택 (Easy / Normal / Hard)  
- 숫자 입력 & 셀 선택 하이라이트  
- 채점 기능 (오답 빨강 하이라이트)  
- 힌트 기능 (광고 없이, 퍼즐당 3회)  
- Undo / Redo (최대 3단계)  
- SettingsPanel: Dark Mode, 테마, BGM/SFX 토글
- StatsPanel: 난이도별 통계 (클리어 횟수, 최고/평균 클리어 타임, 평균 힌트 사용)
- 배너 광고 지원 (Unity Ads)

---

## 📁 프로젝트 구조

```
Assets/
└── Scenes/
    ├── StartScene.unity      # 메인 메뉴, 난이도 선택, Continue, Settings, Stats 버튼
    └── GameScene.unity       # 퍼즐 플레이, Back & Settings & Stats 버튼, 배너 광고

└── Scripts/
    ├── Core/
    │   ├── GameManager.cs      # 새 게임/이어하기 분기, 씬 로드
    │   ├── SaveManager.cs      # 로컬 저장·로드
    │   ├── AdsInitializer.cs   # Unity Ads 초기화
    │   └── BannerAd.cs         # 배너 로드·출력
    │   
    ├── UI/
    │   ├── SettingsManager.cs  # SettingsPanel 토글
    │   ├── StatsManager.cs     # StatsPanel 토글 + 난이도별 통계 갱신
    │   └── ...                 # 기존 StartUIManager, InputManager 등
    │   
    └── Generators/
        └── PuzzleGenerator.cs  # 퍼즐 생성 알고리즘
```

---

## 📅 개발 일정 및 진행 현황

| Day       | 주요 작업                                                                                                                                            |
|-----------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Day 1** | 프로젝트 초기 설정, UI 프리팹 구성, GitHub 커밋                                                                                                         |
| **Day 2** | 퍼즐 생성기 구현, 문제 셀 잠금, SOLID 구조 적용, 난이도 선택 연동                                                                                         |
| **Day 3** | 숫자 입력 & 셀 선택, 채점 기능, 힌트, Undo/Redo, 입력 상태 저장·복원                                                                                     |
| **Day 4** | StartScene UI에 난이도 선택 패널 연동 및 UI 제어, 저장 초기화·Continue 버튼 로직 추가, GameManager 난이도 설정 후 퍼즐 생성 연결                             |
| **Day 5** | Back/Continue 연동, SettingsPanel·StatsPanel UI 뼈대, Unity Ads 배너 연동 및 Canvas 설정                                                                |

