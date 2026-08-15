# Cozy Sound Room — Prompt Structure Standard v1

## Назначение

Этот документ задаёт **единую структуру генерационных промптов** для проекта **Cozy Sound Room**.

Используй его совместно с:

- Project Specification;
- Asset Generation Registry;
- Cozy Sound Room Visual Bible;
- актуальным Handoff;
- референсными изображениями проекта.

Это не конкретный промпт для одной картинки.

Это **система сборки будущих промптов**.

Цель — чтобы все изображения проекта:
- выглядели частью одного мира;
- сохраняли единую художественную логику;
- подходили для production pipeline;
- учитывали Unity и дальнейший compositing;
- не теряли игровые требования ради красивой картинки.

---

# 1. Главный принцип

Каждый prompt собирается из двух частей:

## A. PROJECT DNA

Постоянные признаки Cozy Sound Room, которые почти не меняются:

- художественный стиль;
- материальность;
- уровень стилизации;
- визуальная зрелость;
- базовая цветовая логика;
- отношение к детализации;
- общая эмоциональная атмосфера.

## B. TASK-SPECIFIC BLOCK

То, что относится только к текущему изображению:

- что генерируем;
- ракурс;
- объект;
- состояние;
- сезон;
- освещение;
- background;
- прозрачность;
- required / forbidden elements;
- техническое назначение.

Не переписывай проект заново каждый раз.

Но **ключевые постоянные признаки должны быть встроены в каждый production prompt**, чтобы генератор не потерял стиль.

---

# 2. Universal Prompt Architecture

Для большинства изображений используй следующий порядок блоков.

---

## BLOCK 1 — ASSET TYPE / WHAT TO CREATE

Первая фраза должна сразу сказать генератору, **что он делает**.

Примеры типов:

- master room background;
- interior environment concept;
- isolated game asset;
- animation frame;
- sprite-sheet reference;
- seasonal overlay;
- window exterior background;
- UI element;
- splash-screen artwork.

Не начинать с абстрактных слов вроде:

“a beautiful cozy magical atmosphere…”

Сначала задача, потом атмосфера.

---

## BLOCK 2 — FUNCTION IN THE GAME

Определи назначение изображения.

Например:

- permanent base room;
- isolated interactive prop;
- overlay placed on top of an existing room;
- animation frame that must match previous frames;
- background seen only through the window;
- UI sprite.

Этот блок особенно важен, потому что генератор должен понимать, что изображение является **частью системы**, а не самостоятельной иллюстрацией.

---

## BLOCK 3 — SUBJECT / REQUIRED CONTENT

Чётко описать, что должно присутствовать.

Использовать конкретные существительные.

Например для комнаты:

- one armchair;
- small wooden side table;
- one main book cabinet;
- several separate wall shelves;
- large multi-pane window;
- deep windowsill;
- partial fireplace at edge of frame.

Например для объекта:

- porcelain tea cup;
- matching saucer;
- small metal spoon;
- subtle handmade imperfections.

Не заменять конкретный состав общими словами вроде:
“cozy furniture and decorations”.

---

# 3. COMPOSITION BLOCK

## Для комнаты

Всегда указывать:

**fixed painterly 2D diorama camera, 3/4 corner view, slightly elevated viewpoint**

Дополнительно:

- two walls visible;
- part of the floor visible;
- readable top surfaces;
- readable armchair seat;
- readable windowsill;
- readable shelves;
- generous breathing space;
- central floor relatively open.

### Не использовать

- front-facing flat room;
- one-point photographic interior shot;
- extreme wide-angle lens;
- top-down view;
- true mathematical isometric projection;
- dramatic cinematic perspective.

---

## Для отдельных предметов

Указывать:

- viewing angle;
- orientation;
- visible surfaces;
- silhouette;
- scale;
- contact surface.

Если предмет может стоять на разных поверхностях, предпочитать ракурс, который остаётся естественным после compositing.

---

# 4. ART STYLE BLOCK

Базовая художественная формула проекта:

**70% hand-painted realism, 30% stylized cozy fantasy**

Дополнительная характеристика:

- tactile illustrated rendering;
- natural material readability;
- soft painterly edges;
- believable object construction;
- subtle stylization;
- emotional warmth;
- mature cozy visual language;
- gentle storybook softness without becoming childish.

Фэнтези выражается через:
- reaction;
- animation;
- atmospheric detail.

Не через:
- fantasy architecture;
- exaggerated proportions;
- magical furniture design.

---

# 5. MATERIAL BLOCK

Материалы описывать отдельно и конкретно.

## Базовые материалы мира Cozy Sound Room

### Walls
- warm mineral plaster;
- subtle handmade variation;
- light natural patina.

### Wood
- aged natural oak;
- soft ash-oak;
- muted walnut accents;
- visible natural grain;
- matte finish;
- lightly worn contact edges.

### Fabrics
- linen;
- woven wool;
- cotton;
- matte upholstery;
- tactile natural texture.

### Metal
- aged brass;
- blackened or muted metal;
- soft non-glossy reflections.

### Ceramics
- handmade porcelain;
- matte or softly glazed pottery;
- slight natural irregularity.

### Glass
- soft realistic transparency;
- subtle reflections;
- no synthetic high-gloss CG look.

---

# 6. COLOR BLOCK

Не задавать сцену как случайный набор цветов.

Использовать иерархию проекта.

## Base
- warm ivory;
- oatmeal;
- pale mushroom;
- parchment;
- warm mineral neutral.

## Wood
- oak;
- ash;
- muted walnut;
- soft chestnut.

## Botanical
- sage;
- moss;
- eucalyptus;
- muted olive.

## Accent
- dusty blue;
- muted teal;
- faded terracotta;
- rust;
- ochre.

## Dark anchors
- charcoal;
- deep walnut;
- very dark green;
- muted bronze.

### Общий характер

- low to medium saturation;
- soft contrast;
- calm natural palette;
- no neon;
- no candy colors;
- no monochromatic beige wash.

---

# 7. LIGHTING BLOCK

Свет описывать отдельно от самой комнаты.

Это особенно важно для Cozy Sound Room, потому что время суток меняется системой.

## Base room generation

Не запекать сильное конкретное время суток.

Использовать:

- balanced soft diffuse illumination;
- materials clearly readable;
- neutral enough for later morning/day/evening/night lighting adjustments;
- no permanent amber cast.

---

## Morning

- soft early daylight;
- subtle warm rosy-gold quality;
- gentle long shadows;
- fresh, quiet feeling.

## Day

- diffuse natural daylight;
- neutral warm balance;
- high material readability;
- soft shadow edges.

## Evening

- cooler fading exterior light;
- restrained warm local interior light;
- overlapping soft light pools;
- no dominant ceiling lamp.

## Night

- cool exterior;
- warm protected interior;
- deep but readable shadows;
- calm contrast;
- no crushed black areas.

---

# 8. DETAIL DENSITY BLOCK

Очень важный блок.

Использовать формулу:

**medium-low clutter, medium visual richness**

Для комнаты:

- approximately 60–70% naturally occupied areas;
- approximately 30–40% believable breathing space;
- some empty wall;
- some empty shelf space;
- free windowsill sections;
- open floor areas;
- details grouped in small clusters rather than evenly scattered.

Свободные зоны не должны выглядеть как:
- blank inventory slots;
- missing objects;
- unfinished placeholders.

---

# 9. GAME READABILITY BLOCK

Для всех игровых ассетов явно учитывать:

- clear silhouette;
- readable material;
- identifiable from game-view scale;
- no unnecessary micro-detail;
- no confusing overlapping shapes;
- no baked-in visual effects unless required;
- appropriate separation from surrounding background.

Для комнаты:

- clear horizontal spawn surfaces;
- readable shelves;
- readable table top;
- readable armchair seat;
- readable windowsill;
- enough visible wall space for hanging assets.

---

# 10. BASE ROOM — SPECIAL STRUCTURE

Промпт базовой комнаты должен иметь следующую логику:

## 10.1 What
Permanent master room background.

## 10.2 Architecture
- spacious residential attic;
- gently sloped roof;
- visible wooden beams;
- warm mineral-plaster walls;
- natural wooden floor.

## 10.3 Permanent Furniture
Только:
- one armchair;
- small side table;
- main book cabinet;
- several wall shelves;
- window;
- fireplace architecture.

## 10.4 Camera
Fixed 3/4 elevated diorama view.

## 10.5 Spawn Readiness
Оставить:
- open shelf areas;
- clear table area;
- readable windowsill;
- usable floor space;
- wall space.

## 10.6 Exclusions

Не включать:
- cat;
- tea cup;
- open book;
- wind chimes;
- dreamcatcher;
- knitting basket;
- aquarium;
- vinyl player unless deliberately defined as permanent architecture/furniture in a later approved decision;
- seasonal decor;
- Christmas decoration;
- autumn leaves;
- flowers identifying spring;
- snow effects;
- weather;
- strong time-of-day lighting.

Если статус какого-либо предмета между base и dynamic layer не зафиксирован в источниках, не решать самостоятельно.

---

# 11. SEASONAL LAYER — SPECIAL STRUCTURE

Seasonal layer не является новой комнатой.

Он должен быть сформулирован как:

**additional seasonal decorative layer designed to fit exactly the existing Cozy Sound Room master room without changing architecture, furniture placement, camera or perspective**

---

## Spring

Возможный визуальный язык:

- pale lightweight textile;
- fresh small botanical elements;
- willow / blossom branches;
- young greenery;
- sheer curtain feeling;
- gentle freshness.

---

## Summer

- lighter textile;
- natural woven mat;
- wildflowers;
- berries;
- lighter open window feeling;
- airy curtain.

---

## Autumn

- plaid or woven warm textile;
- terracotta / rust accent;
- maple / rowan branches;
- small pumpkin detail;
- visually denser textile feeling.

---

## Winter

- chunky knitted textile;
- soft wool;
- pine;
- cones;
- cinnamon;
- restrained seasonal greenery;
- frost detail at window.

### Очень важно

Seasonal layer должен:
- сохранять ту же комнату;
- не перемещать мебель;
- не менять геометрию;
- не добавлять огромные декоративные элементы;
- не превращать сезон в тематический праздник.

---

# 12. WINDOW EXTERIOR — SPECIAL STRUCTURE

Window exterior генерируется отдельно от комнаты.

Указывать:

- view designed specifically to be seen through the Cozy Sound Room window;
- perspective compatible with slightly elevated 3/4 interior camera;
- sufficient atmospheric depth;
- no dominant architecture unless explicitly requested;
- natural scenery;
- room remains visual subject, outside world supports it.

Комбинация:

**Season × Time of Day × Weather**

должна рассматриваться как отдельные параметры.

Не смешивать автоматически:

- winter = night;
- autumn = rain;
- summer = sunny;
- spring = morning.

---

# 13. INDIVIDUAL INTERACTIVE OBJECT — SPECIAL STRUCTURE

Структура:

## 13.1 Object
Что именно создаём.

## 13.2 Object Role
Interactive game prop.

## 13.3 Surface Compatibility
На каких surface type объект должен естественно смотреться.

Например:

Cat:
- armchair;
- floor;
- table;
- wide windowsill.

Tea cup:
- table;
- windowsill;
- suitable shelf.

Dreamcatcher:
- wall.

Wind chimes:
- window / suspended.

## 13.4 Visual Style
Тот же hand-painted visual language, что у комнаты.

## 13.5 Lighting
Neutral-compatible lighting unless generating for a specific scene state.

## 13.6 Background
Если ассет должен быть вырезан:
- isolated;
- clean transparent background where generator supports it;
- no cast shadow extending far outside object bounds unless required.

## 13.7 Silhouette
Readable at game scale.

---

# 14. OBJECT STATE SYSTEM

Для анимируемого предмета существует три состояния.

## A. Idle Off

Самое спокойное базовое состояние.

Не использовать активную магию.

Например:

Cat:
- curled up;
- sleeping;
- relaxed.

Tea cup:
- still spoon;
- minimal or no steam depending on production decision.

Fireplace:
- cold logs;
- no flame.

---

## B. Loop Active

Изменение должно быть **небольшим и цикличным**.

Дизайн объекта остаётся тем же.

Изменяется только действие.

Пример:

Cat:
- breathing;
- slight tail movement.

Tea:
- spoon oscillation;
- soft normal steam.

Clock:
- pendulum movement.

---

## C. Secret Reaction

Разовая более выразительная реакция.

Можно добавить небольшой fantasy moment.

Но объект должен оставаться узнаваемым как тот же самый объект.

---

# 15. ANIMATION CONSISTENCY BLOCK

Для последовательности кадров **обязательно** включать требования:

- exact same object design;
- identical proportions;
- identical material;
- identical color palette;
- identical camera;
- identical object orientation;
- identical scale;
- identical lighting direction;
- identical background/transparency;
- only the animated component changes.

Если генератор поддерживает reference image / image-to-image:

использовать предыдущий approved frame или master asset как reference.

---

# 16. UI PROMPT STRUCTURE

UI генерируется отдельно от room art.

Но должен быть родственен ему.

### Material vocabulary

- wood;
- aged brass;
- muted painted metal;
- parchment-like surfaces;
- soft glass;
- handcrafted detail.

### UI requirements

- readable;
- compact;
- low visual aggression;
- game-ready;
- clear states.

Для кнопок:

- Normal;
- Hover;
- Pressed.

Все три состояния:
- same geometry;
- same icon;
- same proportions;
- only feedback state changes.

---

# 17. NEGATIVE PROMPT SYSTEM

Negative prompt должен быть **осмысленным**, а не бесконечным списком популярных ошибок.

Использовать только релевантные запреты.

---

## Universal Cozy Sound Room negatives

В зависимости от генератора можно включать:

- photorealistic interior photography;
- sterile modern luxury interior;
- glossy 3D render;
- generic mobile game asset;
- childish cartoon style;
- chibi proportions;
- exaggerated fantasy architecture;
- neon colors;
- excessive orange lighting;
- excessive clutter;
- maximalist cottagecore;
- perfectly symmetrical showroom composition;
- office interior;
- large work desk;
- sofa;
- wall-to-wall books;
- excessive decorative pattern;
- polished synthetic materials;
- sharp harsh lighting;
- ultra-wide-angle distortion.

Не использовать отрицательные слова, которые конфликтуют с задачей.

---

# 18. PROMPT LENGTH MODES

Prompt-chat должен уметь выдавать три уровня.

---

## SHORT

Для генераторов, которые плохо воспринимают длинные инструкции.

Содержит:
- subject;
- camera;
- key style;
- key materials;
- color;
- 3–5 critical constraints.

---

## STANDARD

Основной рабочий формат.

Содержит:
- asset type;
- function;
- composition;
- required content;
- materials;
- style;
- palette;
- lighting;
- game-readability;
- negative constraints.

---

## PRODUCTION / STRICT

Для сложных изображений и повторной генерации.

Содержит:
- точную композицию;
- hierarchy;
- spawn requirements;
- materials;
- palette;
- object placement rules;
- exclusions;
- technical usage;
- consistency requirements.

---

# 19. PROMPT TEMPLATE — UNIVERSAL

Используй эту внутреннюю схему:

**[ASSET TYPE]**

Create a [specific asset] for the Cozy Sound Room interactive 2D audio-diorama.

**[FUNCTION]**

The image will be used as [base background / isolated prop / overlay / animation frame / UI element].

**[SUBJECT / CONTENT]**

Include:
[specific required content].

**[COMPOSITION / CAMERA]**

[viewpoint, perspective, object orientation, visible surfaces].

**[PROJECT STYLE]**

70% hand-painted realism and 30% restrained stylized cozy fantasy, tactile illustrated materials, believable construction, soft painterly rendering, mature cozy atmosphere.

**[MATERIALS]**

[specific material list].

**[COLOR]**

[palette and saturation].

**[LIGHTING]**

[lighting state].

**[GAME REQUIREMENTS]**

[readability / silhouette / spawn areas / compositing / transparent background].

**[DENSITY]**

[amount of detail / breathing room].

**[CONSISTENCY]**

[reference-image / matching previous approved asset requirements].

**[EXCLUSIONS]**

Do not include:
[relevant forbidden elements].

---

# 20. MASTER ROOM PROMPT TEMPLATE

Использовать этот порядок:

1. Identify it as the **permanent base/master room**.
2. Define camera exactly.
3. Define room as spacious residential attic.
4. Define the two visible walls.
5. Define large multi-pane window.
6. Define armchair.
7. Define small wooden side table.
8. Define book cabinet.
9. Define separate shelves.
10. Define partial fireplace.
11. Define free floor.
12. Define visible spawn-compatible surfaces.
13. Define material palette.
14. Define rendering.
15. Define neutral/adaptable lighting.
16. Explicitly remove dynamic objects.
17. Explicitly remove seasonal elements.

---

# 21. OBJECT PROMPT TEMPLATE

1. Object name.
2. Function in game.
3. Exact design.
4. Materials.
5. Shape/silhouette.
6. Surface compatibility.
7. Camera/view angle.
8. Game scale readability.
9. Project style.
10. Lighting compatibility.
11. Background/transparency.
12. State: Idle / Loop / Secret.
13. Consistency with reference.
14. Exclusions.

---

# 22. ANIMATION PROMPT TEMPLATE

Always specify:

**Reference**

Use the approved Idle object as the exact design reference.

**Preserve**

Preserve exactly:
- object identity;
- proportions;
- construction;
- texture;
- colors;
- camera;
- orientation;
- scale;
- lighting.

**Animate only**

Explicitly list the moving component.

Example:

Animate only:
- tail tip;
- subtle chest breathing.

Everything else remains visually unchanged.

---

# 23. MULTI-FRAME GENERATION

Если генератор создаёт несколько кадров одновременно:

Prompts should specify:

- same object repeated across all frames;
- same canvas size;
- identical scale;
- identical anchor position;
- same perspective;
- only motion phase changes;
- clear sequential motion;
- seamless loop where applicable.

Не просить генератор «нарисовать несколько разных вариантов».

Это должна быть **одна анимация одного объекта**.

---

# 24. REFERENCE IMAGE RULE

Если существует approved reference:

### Reference имеет приоритет над текстовым переосмыслением.

Prompt должен говорить:

- preserve exact visual identity;
- do not redesign;
- do not reinterpret;
- modify only requested element/state.

Особенно важно для:
- animation;
- seasonal layer;
- object variants;
- UI states;
- room iteration.

---

# 25. GENERATOR ADAPTATION

Prompt-chat не должен считать, что один prompt одинаково хорошо работает во всех моделях.

Перед финальной выдачей prompt пользователь может указать генератор.

Тогда адаптировать структуру.

Но **не менять художественную спецификацию проекта**.

Можно менять:
- длину;
- синтаксис;
- weighting;
- negative prompt;
- reference instructions;
- aspect-ratio formatting;
- seed / variation advice.

Нельзя менять:
- room geometry;
- style;
- permanent furniture;
- spawn principles;
- color philosophy;
- visual density.

---

# 26. PROMPT REVIEW CHECKLIST

Перед выдачей любого production prompt проверить:

- [ ] Ясно ли, что именно генерируется?
- [ ] Понятно ли назначение изображения в игре?
- [ ] Не добавлены ли объекты из другого слоя?
- [ ] Соответствует ли камера Visual Bible?
- [ ] Соответствует ли мебель актуальному handoff?
- [ ] Нет ли дивана?
- [ ] Не стал ли стол рабочим desk?
- [ ] Есть ли нужное свободное пространство?
- [ ] Не перегружены ли полки?
- [ ] Не запечено ли случайное время суток?
- [ ] Соответствуют ли материалы Cozy Sound Room?
- [ ] Соответствует ли палитра?
- [ ] Не ушёл ли результат в generic cottagecore / Japandi / Scandi?
- [ ] Интерактивный объект выглядит частью комнаты, а не UI?
- [ ] Для animation сохранён ли exact same design?
- [ ] Есть ли только релевантные negative constraints?

---

# 27. Приоритет документов

При конфликте решений использовать следующую логику:

1. Последнее прямое решение пользователя.
2. Актуальный Handoff.
3. Visual Bible.
4. Project Specification / Asset Registry.
5. Research Report.
6. Общие дизайнерские знания.

Исследование помогает объяснять художественные решения, но **не должно самостоятельно менять уже утверждённую игровую архитектуру проекта**.

---

# 28. Главное правило Prompt Architect

Не пытайся сделать prompt красивее за счёт добавления новых предметов.

Не пытайся сделать сцену уютнее автоматически через:
- больше книг;
- больше свечей;
- больше растений;
- больше пледов;
- больше оранжевого света.

Для Cozy Sound Room уют создаётся через:

**composition + materials + breathing space + tactile detail + nature + visual hierarchy + lighting**

а не через количество cozy props.

---

# 29. Финальная формула

Каждый prompt проекта должен отвечать одновременно на четыре вопроса:

### WHAT IS IT?
Что именно генерируем?

### WHERE DOES IT BELONG?
Какую роль изображение играет в системе игры?

### HOW MUST IT LOOK?
Как оно подчиняется Visual Bible?

### WHAT MUST NOT CHANGE?
Какие элементы проекта должны оставаться стабильными?

Если prompt не отвечает хотя бы на один из этих вопросов, он считается недостаточно точным для production.