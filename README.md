# Unity-GraphView-Editor

本项目使用UnityEditor.UIElements开发，源码参考Unity的ShaderGraph实现，用于实现对话编辑器，当前仅提供编辑器侧。

## 项目特点
- 原生UIElements
- 节点化
- 全局变量可拖拽
- 使用Json存储
- 可扩展
- 源生成器

## 编辑器框架
- Editor（GraphWindow : EditorWindow）
  - GraphEditorView（VisualElement）
    - Toolbar
    - GraphView
      - Manipulators
      - Nodes（BaseNodeView : Node）
        - 见「节点视图」
      - Edges
      - Ports
      - ErrorBadge（IconBadge）
      - SearchWindowProvider
    - Blackboard
      - Fields
        - Drag to GraphView → PropertyNodeView
      - ContextualMenu
  - Persistence
    - GraphObject（ScriptableObject → JSON）
    - BlackboardObject（ScriptableObject → JSON）

## 节点视图

- BaseNodeView（: Node, IDisposable）
  - 公共部分
    - Ports（input / output 容器）
    - Controls
    - Preview
    - Badges
  - ActionNodeView
    - Entry
    - Output
  - BasicNodeView
  - InputView
- PropertyNodeView
