# VR Тренажёр (Unity 6 · URP · XR Interaction Toolkit)

Тестовое задание: VR-тренажёр с лобби и сценой тренировки из 3 групп по 3 шага.
Звуки генерируются кодом, DI-фреймворков и сторонних скриптов нет. Сторонние ресурсы - только текстуры и 3D-модели (CC0): мебель - Kenney Furniture Kit, ящик - Wooden crate, паллета и бочка - 3TD Industrial Pack, консоль - Kenney Space Kit, пьедестал - Quaternius (glb, импортируется пакетом glTFast), текстуры стен/пола/дерева/металла - OpenGameArt (авторство и ссылки в `Assets/_Project/Textures/OpenGameArt/CREDITS.txt` и `Assets/_Project/Models/Furniture/CREDITS.txt`). Предметы сценария (паспорт, сканер, папка, ящики, печать) собраны из примитивов.

## Архитектура

Проект разделён на независимые слои, которые общаются **только событиями (C# `event`)**.
*Сцена* (цели `ScenarioTarget`: `ReachTarget`, `GrabTarget`, `ClickTarget`, `UIButtonTarget`) знает только,
**когда** пользователь что-то сделал, и сообщает об этом в `InteractionHub` (`ActionType` + `targetId`).
*Логика* (`ScenarioRunner`) подписана на хаб и ничего не знает про XR, UI и звук: она сверяет действие с
ожидаемым в текущем шаге, определяет успех, ошибку (не та точка/объект/кнопка) или нарушение порядка
(действие из более позднего шага группы → группа закрывается, незавершённые шаги получают статус «Пропущен»),
и публикует события `GroupStarted / StepActivated / ActionExpected / StepFinished / ViolationDetected /
GroupFinished / ScenarioFinished`. *Презентеры* (`TrainingHudPresenter`, `ResultsPresenter`,
`FeedbackAudioPresenter`, `TargetHintPresenter`) только подписываются на эти события: HUD с инструкцией и
статусами шагов, итоговый экран (Restart / Возврат в Лобби), звуки успеха/ошибки и подсветка ожидаемой цели.
Сценарий - это данные (`ScenarioDefinition`, ScriptableObject: группы → шаги → список `ExpectedAction`),
цели привязываются по строковому id, поэтому новые шаги и группы добавляются без правки кода (принципы
SOLID: runner закрыт для изменений, расширяется новыми целями/презентерами; подсветка скрыта за
`IHighlightable`).

**Подсветка.** Платный Highlight Plus нельзя использовать по условию «без готовых ассетов», поэтому реализован
собственный Outline: шейдер `OutlineHull` (inverted hull, URP) + `OutlineHighlight` (3D) и
`UIOutlineHighlight` (кнопки). Они стоят за интерфейсом `IHighlightable`, так что Highlight Plus
подключается заменой одного компонента. **Перемещение**: скольжение (`DynamicMoveProvider`) и телепортация
(`TeleportationArea` на полу) из стартового рига XRI. **UI** - world-space Canvas с `TrackedDeviceGraphicRaycaster`
(контроллеры) и `GraphicRaycaster` (мышь), `XRUIInputModule`.

## Запуск

* Unity **6000.6.8f1**, URP 17.6, XR Interaction Toolkit 3.7.
* Сцены: `Assets/_Project/Scenes/Lobby.unity` (стартовая), `Training.unity`.
* С шлемом (PC VR): OpenXR уже включён в *Project Settings → XR Plug-in Management → Windows*, профили контроллеров Quest/Index/Vive/WMR включены. Нужно: подключить шлем (Quest Link / Air Link / SteamVR), в настройках выбрать его OpenXR runtime активным, открыть `Lobby.unity` и нажать Play. Игра продолжает работать, когда окно редактора не в фокусе (Run In Background).
* Управление в шлеме: левый стик - скольжение, правый стик - поворот, толчок стика вперёд - телепорт, курок - нажатие кнопок UI и клик лучом по предмету, боковая кнопка (grip) - взять предмет (стандартные биндинги XRI, grip = Select, курок = Activate).
* Если в шлеме чёрный экран или ошибки графики: Player Settings → Other Settings → Graphics APIs for Windows, поставить Direct3D11 первым и перезапустить редактор.
* Без шлема: `SimulatorBootstrap` создаёт **XR Interaction Simulator** (WASD + мышь; подсказки управления - в левом нижнем углу игрового окна).

## Структура

```
Assets/_Project/
  Models/ Textures/  CC0 модели и текстуры
  Scripts/{Core, Scenario/{Data,Runtime,Targets,Presenters}, Highlight, Audio, UI}
  Shaders/       OutlineHull.shader
  Materials/ Prefabs/{Core,UI,World}/ Data/ Scenes/
Assets/Samples/  стартовые ассеты XRI (риг, контроллеры, input actions, симулятор)
```

## Видение гибкой системы сценариев

Текущая модель «группа → линейные шаги → действия» описана **данными**, а условия шага - типом действия
(`ActionType`) и id цели. Для максимальной гибкости следующий шаг эволюции - заменить enum на полиморфные
`StepCondition` (ScriptableObject/`[SerializeReference]`): `AllOf / AnyOf / Sequence / Timer / Custom`, а
правила реакции на ошибку (завершить шаг, дать вторую попытку, закрыть группу) вынести в `FailurePolicy` на
уровне шага или группы. Тогда ветвления, необязательные шаги, таймеры и «штрафные» очки добавляются как новые
классы-данные без изменения `ScenarioRunner` (Open/Closed), а сценарии можно импортировать из JSON (формат из
ТЗ ложится на `GroupDefinition/StepDefinition` один к одному) и редактировать дизайнерами без программиста.
