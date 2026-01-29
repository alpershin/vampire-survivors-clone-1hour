# Vampire Survivors Clone (Prototype)

## Что это
- Top-down выживалка с автокастом: движение игрока, волны врагов, опыт, апгрейды.
- Баланс во внешнем `StreamingAssets/balance.json` (статы игрока, врагов, способностей).

## Архитектура (скрипты)
- Конфиги: `ConfigManager` (синглтон) грузит JSON; модели в `Config/BalanceModels.cs`.
- Игрок/камера: `PlayerController` (Legacy Input, kinematic Rigidbody2D, смерть → рестарт сцены); `CameraFollow` (Lerp, фиксированный Z).
- Бой: `IDamageable`; `BaseAbility`; `ProjectileAbility` (спред по уровню, пул снарядов); `AuraAbility` (радиус урона = визуал); `Projectile` (направление, время жизни, игнорирует владельца); `AbilityManager` (стартует Projectile lvl1, апгрейды из UI).
- Враги: `EnemyAI` (преследование, стоп по AttackRange, удар по кулдауну); `EnemySpawner` (волны, пул по типам, дроп XP-гемов); `ObjectPool` (универсальный пул).
- Прогрессия: `ExperienceManager` (XP/level, события); `ExperienceGem` (OnTrigger → XP).
- UI: `UIManager` (бары HP/XP, LevelUp меню паузит игру, спавнит `UpgradeCard`); `UpgradeCard` (имя/описание/уровни, клик → апгрейд).

## Настройка сцены (кратко)
1) Помести `ConfigManager`, `ExperienceManager`, `UIManager` в сцену; привяжи в UI ссылки на бары/тексты, `upgradeCardPrefab`, `upgradesContainer`.
2) Игрок: `PlayerController` + Rigidbody2D (Kinematic) + Collider2D; повесь `AbilityManager`, `ProjectileAbility`, `AuraAbility` (enemyLayer = слой врагов, спрайт круга в `auraVisual`).
3) Камера: `CameraFollow` с Target = игрок.
4) Враги: префабы с `EnemyAI` (AttackRange из баланса), слой врагов; `EnemySpawner` со списком типов/префабов, `experienceGemPool`.
5) XP-гем: префаб `ExperienceGem` с trigger-коллайдером и пулом.
6) В Input Manager оставь оси `Horizontal/Vertical`.

## Баланс
- Правится в `Assets/StreamingAssets/balance.json`: PlayerStats, EnemyStats (HP/Speed/Damage/XP/AttackRange), AbilityStats (Damage/Cooldown/Area/Speed/ProjectileCount/Description).

## Известные моменты
- Способности активируются только после `AbilityManager.Configure` (по умолчанию Projectile lvl1 включен).
- Для урона по врагам слои должны совпадать с `enemyLayer` у Projectile/Aura.
- При смерти игрока сцена перезапускается (нужна запись в Build Settings).
