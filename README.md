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
- 입력 상태 저장·복원 (앱 재실행/씬 재진입)  
- 배너 광고 지원 (Unity Ads)

---

## 📁 프로젝트 구조

```
Assets/
└── Scripts/
    ├── Core/                  
    │   ├── GameManager.cs
    │   ├── SaveManager.cs
    │   ├── IPuzzleGenerator.cs
    │   └── Difficulty.cs
    │   
    ├── Generators/            
    │   └── PuzzleGenerator.cs
    │   
    ├── Cells/                 
    │   └── PuzzleCell.cs
    │   
    └── UI/                    
        ├── InputManager.cs
        ├── PuzzleManager.cs
        ├── HintManager.cs
        └── DifficultySelector.cs
```

---

## 📅 개발 일정 및 진행 현황

| Day       | 주요 작업                                                   |
|-----------|-------------------------------------------------------------|
| **Day 1** | 프로젝트 초기 설정, UI 프리팹 구성, GitHub 커밋            |
| **Day 2** | 퍼즐 생성기 구현, 문제 셀 잠금, SOLID 구조 적용, 난이도 선택 연동 |
| **Day 3** | 숫자 입력 & 셀 선택, 채점 기능, 힌트, Undo/Redo, 입력 상태 저장·복원 |

---

## 💡 향후 개선

- Google Play 출시 준비 (아이콘, 스토어 등록)  
- 광고 최적화 & 수익화  
- 통계 및 리더보드 추가  
- UI/UX 디테일 업그레이드
