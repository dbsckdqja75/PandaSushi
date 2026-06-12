# PandaSushi

<div align="center">
  <img src="https://github.com/user-attachments/assets/75d79282-d651-4cb7-9b03-25a11d2ee744" width="49%" height="49%"/>
  <img src="https://github.com/user-attachments/assets/32f7e9dc-2733-47f0-9e65-8dd71343b275" width="49%" height="49%"/>
  <img alt="Image Sequence_035_0025" src="https://github.com/user-attachments/assets/648e289f-4ff4-4b58-8594-2c9efed874c4" width="49%" height="49%"/>
  <img alt="Image Sequence_037_0000" src="https://github.com/user-attachments/assets/0d68d07f-b3ec-408f-a7ff-17e72d2f4622" width="49%" height="49%"/>
</div>

<br>
Unity로 개발한 <b>쿠킹 시뮬레이터 게임 프로젝트</b>입니다.<br>

<br>
판다 할아버지를 대신하여 가게를 잠깐(?) 맡게된 플레이어는<br>
여러 레시피들을 요리하고 재료들을 조합하며, 까다롭고 재밌는 돌발 상황들도 마주하면서<br>
식당을 운영하여 종합 리뷰 별점 5개를 채우는 것이 게임의 목표입니다.<br>

<br><br>

+ 개발 기간 : 2026.03 ~ 2026.05
+ 개발 인원 : 2인
+ 타겟 플랫폼 : Windows, macOS, Linux/SteamOS

<br>

***

## 팀원
| 윤창범 | 이상화 | 
|:---:|:---:|
| <img src="https://avatars.githubusercontent.com/u/22255667?v=4" width="120" height="120"/> | <img src="https://avatars.githubusercontent.com/u/83414122?v=4" width="120" height="120"/> | 
| 프로그래밍 / 3D 모델링 / UI 디자인 | 2D 아트 / UI 디자인 | 

<br>

## 개발 환경
+ Unity (6000.3.7f1 LTS)
+ JetBrains Rider (2025.2.2.1)
+ C#
+ Windwos / macOS

<br>

## 주요 기술
| 기술 |  |
|:---:|:---|
| 싱글톤 패턴 | [MonoSingleton&lt;T&gt;](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Pattern/MonoSingleton.cs) 구현으로 주요 매니저 클래스 관리 <br> [StageManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/StageManager.cs), [SoundManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/SoundManager.cs), [CurrencyManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/CurrencyManager.cs), [LocalizationManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/LocalizationManager.cs) |
| 상태 패턴 | 분할 클래스 구현으로 [StageManager.State](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/StageManager.State.cs)를 통해 게임의 흐름을 [EGameState](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Enum/EGameState.cs)에 따라 상태별 로직 관리 |
| 이벤트 기반 아키텍처 | [EventManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/EventManager.cs)와 [PandaEvent](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/PandaEvent.cs) 구현으로 클래스 간의 의존성을 낮추고 <br> [EGameEvent](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Enum/EGameEvent.cs)로 게임 상태 변화와 업데이트를 이벤트로 실시간 관리 |
| 오브젝트 풀링 | [ObjectPool](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/ObjectPool.cs) 구현으로 손님, 라이더, FX 등 자주 생성되고 파괴되는 객체들은 재사용 관리 |
| 데이터 저장 & 암호화 | 중요 변수 또는 저장 데이터들을 [PlayerPrefsManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/PlayerPrefsManager.cs), [EncryptAES](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Extension/EncryptAES.cs) 구현으로 **AES암호화**하여 관리 |
| 레시피 데이터 관리 | **ScriptableObject** 기반으로 [RecipeData](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Data/RecipeData.cs), [IngredientData](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Data/IngredientData.cs), [MixData](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Data/MixData.cs)를 구현하여 레시피/재료/조합 정보 관리 |
| 사운드 관리 | [SoundManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/SoundManager.cs) 구현으로 인게임의 모든 BGM과 SFX 리소스 풀링 관리 및 Coroutine 기반으로 <br> Volume, Mute, CrossFade 제어 |
| 리소스 관리 | [PandaResources](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/PandaResources.cs) 구현으로 프리팹, 사운드, 아이콘 등의 자주 로드되는 리소스들을 참조하도록 관리 |
| 다국어 체계 | **Unity Localization** 패키지 기반으로 [LocalizationManager](https://github.com/dbsckdqja75/PandaSushi/blob/main/Assets/02.%20Scripts/Core/LocalizationManager.cs) 구현 및 여러 텍스트/이미지 언어별 동적 관리 |

<br>

## 구현 기능 (세부 내용)

!!! 작성 중 !!!

<details>
  <summary>플레이어</summary>
</details>

<details>
  <summary>손님</summary>
</details>

<details>
  <summary>카메라</summary>
</details>

<details>
  <summary>상호작용</summary>
</details>

<details>
  <summary>요리</summary>
</details>

<details>
  <summary>재고 관리/인테리어</summary>
</details>

<details>
  <summary>UI 관리/제어</summary>
</details>

<details>
  <summary>게임패드 조작/가이드 구현</summary>
</details>

<br>

## 프로젝트 설계 시각적 구조 (다이어그램)
```mermaid
graph TD
    subgraph Core_Managers [주요 매니저 - MonoSingleton]
        SM[StageManager]
        EM[EventManager]
        OM[OrderManager]
        CM[CanvasManager]
        SoM[SoundManager]
        CurM[CurrencyManager]
        LM[LocalizationManager]
        PR[PandaResources]
    end

    subgraph Data_Layer [데이터 레이어 - ScriptableObject]
        RD[RecipeData]
        ID[IngredientData]
        MD[MixData]
        GS[GlobalState]
    end

    subgraph Interactors [상호작용 객체]
        PT[PrepTable]
        OV[Oven]
        FR[Fridge]
        ST[ServeTable]
        SK[Sink]
    end

    subgraph Entities [플레이어/NPC 객체]
        PL[Player]
        CU[Customer]
        RI[Rider]
    end

    subgraph UI_Layer [UI 레이어]
        OUI[OrderUI]
        CUI[CurrencyUI]
        NUI[NotificationUI]
        RB[RecipeBook]
    end

    SM -->|게임 상태 제어| EM
    EM -->|이벤트 호출| OM
    EM -->|이벤트 호출| CU
    EM -->|이벤트 호출| PL

    OM -->|주문 정보 조회| RD
    RD -->|참조| ID

    PL -->|상호작용| Interactors
    Interactors -->|데이터 참조| ID
    Interactors -->|요리 완료 콜백 처리| OM

    OM -->|UI 업데이트| OUI
    CurM -->|UI 업데이트| CUI
    
    SoM -->|사운드 출력| Entities
    
    PR -->|리소스 로드| Data_Layer
    PR -->|리소스 로드| Entities
```

프로젝트는 기본적으로 **매니저 패턴**과 **이벤트 기반 아키텍처**를 토대로 설계했습니다.<br>
<br>
자주 발생되어 호출되는 메서드들은 EventManager를 통해 관리 및 호출할 수 있도록 구현했고,<br>
핵심이 되는 주요 매니저 클래스들만 MonoSingleton을 통해 전역적으로 접근할 수 있도록 구현했습니다.

초기에 설계를 확정짓고 진행한 구조가 아닌, 실제로 구현을 하면서 플레이 환경 규모를 생각했을때<br>
현재의 구조가 가장 적합하고 수정이 용이한 설계라고 생각하여 그대로 채택하였습니다.

<br><br>

## 게임 다운로드
### <a href="https://github.com/dbsckdqja75/PandaSushi/releases/download/v1.3/PandaSushi_Windows.zip">Github 배포 파일 (Windows)</a>
### <a href="https://team-campfire.itch.io/pandasushi">itch.io 배포 페이지</a>

