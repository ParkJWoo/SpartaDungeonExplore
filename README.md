# Sparta Dungeon Explore README

* * *

## 1. 개요

- 구현 위주로 진행한 프로젝트이기에 게임적으로 즐길 요소들이 없습니다.
- ** Main 브런치 내 제가 구현한 기능들을 확인할 수 있습니다. **

### 해당 리포지터리 내 Git Commit Convention 적용
![image](https://github.com/user-attachments/assets/8b0b2b06-5caa-49ec-a50a-dabc5a5002f3)

* * *

## 2. Manager 객체들 설명

### 2.1. CharacterManager | Scripts 폴더 → 0.Managers 폴더 → CharacterManager.cs
- 강의 영상과 동일하게 작업하였습니다.


### 2.2. GameMediator | Scripts 폴더 → 0.Managers 폴더 → GameMediator.cs
![image](https://github.com/user-attachments/assets/62919141-70c5-4616-9726-2c7676b0099d)
![image](https://github.com/user-attachments/assets/ea2be80b-3b85-4dda-a5bb-95250504d8da)
- C# 디자인 패턴인 "중재자 패턴"을 적용한 스크립트입니다.
- Notify 메서드를 통해 플레이어가 스태미너를 사용하거나, 점프력 상승, 이동 속도 증가 등 아이템 효과 적용 내용을 해당 메서드를 통해서만 진행되도록 구현하였습니다.
- DonDestroyOnLoad를 사용하여 중재자를 싱글톤화 처리하였습니다. 

* * *

## 3. 필수 기능 구현 

### 3.1. 기본 이동 및 점프, 대쉬 기능 구현 | Scripts 폴더 → 1. Player 폴더 → PlayerController.cs
![image](https://github.com/user-attachments/assets/2ebcc81f-7c96-4c57-aaa8-4909bcc0f011)
![image](https://github.com/user-attachments/assets/5ab6d2f5-9339-42e1-a5b2-c0064c5de038)
![image](https://github.com/user-attachments/assets/f651003c-8748-4877-8d51-a07491215e49)
![image](https://github.com/user-attachments/assets/06e3f232-bab4-470b-ad88-4f698980b473)

- InputSystem을 활용하여 WASD를 눌러 이동, Space 바를 눌러 점프, 왼쪽 Shift 키를 누르면서 이동 중에 대쉬하는 기능을 구현하였습니다.
- 점프, 대쉬 등 스태미너를 소모하는 기능이 수행될 경우, GameMediator.Notify 메서드를 통해 스태미너를 소모할 것임을 알리도록 구현하였습니다.
- 점프와 대쉬의 경우, 사용할 때마다 지정된 스태미너를 소모하며, 플레이어가 보유한 스태미너가 필요 스태미너량보다 적을 경우, 시행되지 않도록 구현하였습니다.

***

### 3.2. 체력바 UI, 스태미너 UI 표시  | Scripts 폴더 → 2.UIs 폴더 → Condition.cs / 1. Player 폴더 → PlayerCondition.cs
![image](https://github.com/user-attachments/assets/250dfa2e-b50e-410c-ad1e-36d776475402)
![image](https://github.com/user-attachments/assets/77f688df-03ed-4ad8-83c1-9dcd9fc54784)
![image](https://github.com/user-attachments/assets/5f1fb676-e4d0-4c15-8b2c-8fd49e5b0e3d)
![image](https://github.com/user-attachments/assets/7ea68394-ee7b-443b-bd6d-0373f40199a2)

- Condition.cs를 통해 각 UI에 세팅할 값들을 적용하고, UI 상태 변화 메서드를 관리합니다.
- PlayerCondition.cs를 통해 인게임 상에서 플레이어의 상태 변화에 맞춰 UI 상태 변화 메서드를 호출하여 적용합니다.

***

### 3.3. 동적 환경 조사 | Scripts 폴더 → 1. Player 폴더 → PlayerInteraction.cs
![image](https://github.com/user-attachments/assets/efca8150-d16e-4ef4-883f-1dd089ccc515)
![image](https://github.com/user-attachments/assets/c0df9062-1209-48d5-99ef-b93c1d7e83f4)
![image](https://github.com/user-attachments/assets/13799119-30f2-4c57-8401-d5d23d638ccf)

- 게임 내 플레이어 하위에 적용된 메인 카메라를 기준으로 쏘는 Ray에 특정 오브젝트가 부딪혔을 경우, 화면 하단 부분에 해당 오브젝트에 대한 설명 문구가 나오도록 구현하였습니다.

***

### 3.4. 점프대 구현 | Scripts 폴더 → 4. Objects 폴더 → JumpPad.cs
![bandicam 2025-05-22 15-34-49-489](https://github.com/user-attachments/assets/f9aa56f6-10cd-4507-a470-8128006867b0)
![image](https://github.com/user-attachments/assets/b0fe243c-0b9a-4ed6-a141-2e9d169a2829)
![image](https://github.com/user-attachments/assets/c425e3ed-ac6d-41d5-826a-ca134e4f0eae)

- 게임 내 작은 큐브 형태로 이동하여 충돌할 때, 지정된 jumpPower만큼 플레이어를 y축으로만 이동시켜주는 로직을 사용하였습니다.

***

### 3.5. ScriptableObject를 사용하여 아이템 데이터 구현하기 |  Scripts 폴더 → 96. ScriptableObject 폴더 → Data 폴더 내 있는 스크립트들
![image](https://github.com/user-attachments/assets/7c0d538f-d380-427b-8616-7eae4c844c92)
![image](https://github.com/user-attachments/assets/6c3c65ab-0cb0-4cfd-bcea-f34c26711bbf)

- ScriptableObject를 사용하여 ItemData 스크립트를 제작 후, 유니티 에디터 상에서 만들고자 하는 아이템을 생성하여 데이터를 적용할 수 있도록 구현하였습니다.

***

### 3.6. Coroutine을 사용하여 아이템 사용 기능 구현하기 | Scripts 폴더 → 2. UIs 폴더 → UIInventory.cs / Scripts 폴더 → 1. Player 폴더 → PlayerInteraction.cs 
![bandicam 2025-05-22 15-48-17-286](https://github.com/user-attachments/assets/e21d7b61-46e3-40f8-abf1-829f1c07883d)
![image](https://github.com/user-attachments/assets/1d0337c4-02a8-4276-9c01-3a30fd3c970f)
![image](https://github.com/user-attachments/assets/df0b7de4-409a-41c5-9f8f-7a5f0f2c1376)
![image](https://github.com/user-attachments/assets/7dfb6220-52fe-400b-b1da-fcc8ba3287d9)

- 인게임에서 플레이어의 정면에 있는 아이템을 E키를 눌러 획득하는 기능을 구현하였습니다.
- Tab키를 눌러 인벤토리 UI 창을 킨 후, 아이템을 선택하여 [사용하기] 버튼을 누를 시 해당 아이템을 인벤토리 창에서 제거 및 아이템 효과가 적용되도록 구현하였습니다.

***

## 4. 도전 기능 구현

### 4.1. 벽 타기 기능 구현 | Scripts 폴더 → 1. Player 폴더 → PlayerController.cs
![bandicam 2025-05-22 15-57-57-206](https://github.com/user-attachments/assets/03129572-ce1f-4840-a2e2-7b35bed59af9)
![image](https://github.com/user-attachments/assets/d46db102-06ff-4b10-9f05-0b1be2cbda2f)
![image](https://github.com/user-attachments/assets/3ddc68bc-69a8-43e8-9333-24ce03cc8ede)

- Layer가 Wall로 설정되어 있는 오브젝트를 향해 이동 키를 입력 시, 해당 오브젝트를 타고 올라가는 기능을 구현하였습니다.
- 벽을 타는 도중에 이동 키를 입력하지 않을 경우, 밑으로 떨어지도록 구현하였습니다.
- 벽으로 이동 시, 벽을 통과하는 경우가 발생할 수 있습니다.

***

### 4.2. 장비 장착 기능 구현 | Scripts 폴더 → 2. UIs 폴더 → UIInventory.cs / Scripts 폴더 → 3. Items 폴더 → Equipment.cs
![bandicam 2025-05-22 16-07-58-967](https://github.com/user-attachments/assets/6aca87fa-2ab1-4317-8c34-8b03c1d50c99)
![image](https://github.com/user-attachments/assets/48ddcc11-b4d8-4de7-b3d1-2f463ae584c8)
![image](https://github.com/user-attachments/assets/fba57c86-a00d-449b-a100-e8dfd9fa9081)

- 인게임에서 플레이어의 정면에 있는 아이템을 E키를 눌러 획득하는 기능을 구현하였습니다.
- Tab 키를 눌러 인벤토리 UI 창을 킨 후, 장비 아이템을 선택하여 [장착하기] 버튼을 눌러 아이템을 장착하는 기능을 구현하였습니다.
- Equipment.cs → EquipNew 메서드를 통해 아이템이 가지고 있는 능력치 상승치를 플레이어의 스탯에 적용하도록 구현하였습니다.
- Tab 키를 눌러 인벤토리 UI 창을 킨 후, 장비 아이템을 선택하여 [해제하기] 버튼을 눌러 장착한 아이템을 해제하는 기능을 구현하였습니다.
- Equipment.cs → UnEquip 메서드를 통해 장착한 아이템이 가지고 있는 능력치 상승된 부분을 플레이어의 스탯에서 빼도록 구현하였습니다.

***

# EOD
