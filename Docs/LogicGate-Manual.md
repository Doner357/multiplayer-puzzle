# Logic Gate 系統手冊

## 概述

Logic Gate 系統將訊號（如壓力板）連接到動作（如門）。它檢查條件並在條件滿足或不滿足時觸發事件。

## 組件

### NetworkSignal

網路同步的布林狀態。可以是 `true`（啟動）或 `false`（未啟動）。

- 附加在觸發源上（例如 PressurePlate 根物件）
- 自動在網路上同步

### Condition

每個條件有兩個欄位：

| 欄位 | 說明 |
|------|------|
| `Signal` | NetworkSignal 的參考 |
| `Expected State` | 此條件被視為「滿足」所需的狀態 |

### LogicGate

評估所有條件並觸發事件：

- `OnAllConditionsMet` - 當所有條件都符合其預期狀態時觸發
- `OnConditionsNotMet` - 當任何條件不符合時觸發

## Expected State 說明

`Expected State` 勾選框決定條件何時被視為「滿足」。

### Expected State = TRUE（有勾選）

當訊號為**啟動**狀態時，條件滿足。

| 訊號狀態 | 條件滿足？ |
|----------|-----------|
| true（啟動） | 是 |
| false（未啟動） | 否 |

**使用情境**：當某事正在發生時觸發動作。

範例：壓力板被踩下時開門。

### Expected State = FALSE（未勾選）

當訊號為**未啟動**狀態時，條件滿足。

| 訊號狀態 | 條件滿足？ |
|----------|-----------|
| true（啟動） | 否 |
| false（未啟動） | 是 |

**使用情境**：當某事沒有發生時觸發動作。

範例：壓力板沒被踩時保持開門（反向邏輯）。

## 常見模式

### 模式 1：按住開門

設定：
- Signal: PressurePlate
- Expected State: TRUE（勾選）
- OnAllConditionsMet: Door.Open
- OnConditionsNotMet: Door.Close

結果：
```
沒人在板上 -> 門關閉
踩上壓力板 -> 門打開
離開壓力板 -> 門關閉
```

### 模式 2：按住關門（反向）

設定：
- Signal: PressurePlate
- Expected State: FALSE（未勾選）
- OnAllConditionsMet: Door.Open
- OnConditionsNotMet: Door.Close

結果：
```
沒人在板上 -> 門打開
踩上壓力板 -> 門關閉
離開壓力板 -> 門打開
```

### 模式 3：多條件（AND 邏輯閘）

設定：
- Condition 0: PressurePlate_A, Expected State: TRUE
- Condition 1: PressurePlate_B, Expected State: TRUE
- OnAllConditionsMet: Door.Open

結果：
```
兩個板都被踩 -> 門打開
任一板放開   -> 門關閉
```

### 模式 4：混合條件

設定：
- Condition 0: PressurePlate_A, Expected State: TRUE
- Condition 1: PressurePlate_B, Expected State: FALSE
- OnAllConditionsMet: Door.Open

結果：
```
A 被踩 且 B 沒被踩 -> 門打開
其他情況          -> 門關閉
```

## 疑難排解

**門的行為與預期相反**
- 切換 Expected State 勾選框

**踩壓力板沒反應**
- 確認 NetworkSignal 在 PressurePlate 根物件上
- 確認 LogicGate 有正確的 Signal 參考
- 確認 PressurePlateBehaviour 的 targetTag 設定正確

**在編輯器可用但建置後不行**
- 確保所有物件都有 NetworkObject 組件
- 邏輯只在伺服器端執行，確保你是 Host
