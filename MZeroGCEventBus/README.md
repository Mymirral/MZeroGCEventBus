# MZeroGCEventBus - Unity零GC事件总线

一个专为Unity设计的高性能、零GC分配的事件总线系统，适用于需要高性能和低GC压力的游戏开发场景。

## 特点

- **零GC分配**：使用结构体和预分配数组，避免在事件处理过程中产生垃圾
- **类型安全**：基于泛型实现，确保事件类型匹配
- **高效订阅/发布**：O(1)复杂度的订阅和取消订阅操作
- **轻量级**：核心代码简洁，无外部依赖

## 安装（通过UPM）

在Unity编辑器中，打开`Window > Package Manager`，点击右上角`+`按钮，选择`Add package from git URL...`，输入以下地址：

```
https://github.com/Mymirral/MZeroGCEventBus.git?path=MZeroGCEventBus
```

## 使用示例

### 1. 定义事件类型

```csharp
// 自定义事件数据结构（建议使用struct减少GC）
public struct PlayerMoveEvent
{
    public float X;
    public float Y;
    public float Speed;
}
```

### 2. 创建事件监听器

```csharp
using MZeroGCEventBus.Core;
using MZeroGCEventBus.Core.Interface;

public class PlayerMoveListener : IMListener<PlayerMoveEvent>
{
    private MEventHandle _handle;

    public void Initialize()
    {
        // 订阅事件并保存句柄用于后续取消订阅
        _handle = MEventBus.Subscribe(this);
    }

    public void OnEvent(in PlayerMoveEvent e)
    {
        // 处理事件（注意：e是in参数，避免复制）
        Debug.Log($"玩家移动: X={e.X}, Y={e.Y}, Speed={e.Speed}");
    }

    public void Cleanup()
    {
        // 取消订阅
        MEventBus.Unsubscribe<PlayerMoveEvent>(_handle);
    }
}
```

### 3. 发布事件

```csharp
// 在游戏逻辑中发布事件
var moveEvent = new PlayerMoveEvent
{
    X = 10f,
    Y = 5f,
    Speed = 2.5f
};

MEventBus.Publish(moveEvent);
```

### 4. 清理所有订阅（可选）

```csharp
// 重置特定事件类型的所有订阅
MEventBus.Clear<PlayerMoveEvent>();
```

## 工作原理

- **MEventHandle**：作为订阅凭证，包含索引和版本号，确保安全取消订阅
- **ListenerSlot**：使用数组存储监听器，通过交换删除实现O(1)复杂度的取消订阅
- **版本控制**：防止使用过期的句柄进行取消订阅操作

## 最佳实践

1. 始终在对象销毁时调用`Unsubscribe`，避免内存泄漏
2. 对于频繁触发的事件，建议使用`struct`作为事件数据类型
3. 避免在`OnEvent`中执行耗时操作，以免影响性能

## 技术细节

- 默认初始容量：128个监听器
- 容量不足时自动扩容（2倍增长）
- 事件发布时遍历所有监听器，时间复杂度O(n)

> 本项目由[Mymirral](https://github.com/Mymirral)开发维护