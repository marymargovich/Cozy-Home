# Cozy Sound Room — Prompt Structure Standard v2

## Назначение

Этот документ задаёт рабочую систему для **чата, который пишет промпты**, а не для image generator напрямую.

Главный урок после тестовых генераций master room:

> **Prompt Architect должен думать широко по всем источникам проекта, но image generator должен получать короткий, пространственно конкретный art-direction brief.**

Не склеивать Visual Bible, Project Specification и Asset Registry в один гигантский prompt.

---

# 1. Два уровня работы

## LEVEL A — INTERNAL DESIGN SPEC

Prompt Architect обязан учитывать:
- Visual Bible;
- Project Specification;
- Asset Registry;
- актуальные прямые решения пользователя;
- approved reference images;
- spawn logic;
- layer separation;
- animation consistency;
- game readability.

Этот уровень нужен для анализа и проверки решений.

## LEVEL B — GENERATOR PROMPT

Image generator получает только то, что может непосредственно визуально выполнить:
1. что создать;
2. reference usage;
3. camera / framing;
4. spatial layout;
5. exact objects;
6. clear/empty required surfaces;
7. style/materials;
8. palette;
9. lighting;
10. 5–8 критичных exclusions.

Не передавать генератору лишние объяснения Unity pipeline, DSP, будущих систем и общую философию, если это не меняет конкретное изображение.

---

# 2. Project DNA — только релевантное

Базовый визуальный DNA:
- **Soft Woodland Atelier**;
- 70% hand-painted realism / 30% restrained cozy stylization;
- tactile natural materials;
- believable construction;
- mature cozy warmth;
- low-to-medium saturation;
- soft contrast;
- medium-low clutter / medium visual richness;
- believable architecture;
- fantasy through behavior, not architecture.

В production prompt включать только те признаки DNA, которые реально влияют на текущий кадр.

---

# 3. Reference Image Rule

Если есть approved reference, reference имеет приоритет для геометрии и identity.

Для room-generation формулировать так:

**Use the provided reference strictly as the guide for composition, camera, room geometry, architectural proportions, and furniture placement. Do not copy its interior style, colors, material treatment, or decorative character unless explicitly approved. Apply the Cozy Sound Room Visual Bible instead.**

Для approved master room reference:
- preserve the existing room composition;
- preserve camera;
- preserve permanent furniture placement;
- preserve window/fireplace side placement;
- modify only requested layer/state.

---

# 4. Positional Language — обязательно для комнаты

Не ограничиваться списком объектов. Всегда писать **что + где**.

Примеры:
- `at the far left edge`;
- `in the left-center area`;
- `immediately to the right of the armchair`;
- `slightly right of center`;
- `on the right wall`;
- `partially cropped by the frame edge`.

Для master room positional layout должен описываться до style/material blocks.

---

# 5. Approved Master Room Layout

Текущий базовый layout:
- partial fireplace at far left edge;
- broad sage armchair in left-center;
- small side table immediately to the right;
- medium-height book cabinet slightly right of center;
- several sparse floating shelves;
- large multi-pane window on the right wall;
- deep windowsill;
- open central floor;
- warm mineral plaster;
- restrained beams;
- aged light-oak floor;
- soft neutral daylight.

При генерации room variations не переизобретать layout без прямой задачи.

---

# 6. Spawn-Compatible Surfaces

Вместо технического описания spawn-system использовать визуальные требования:

- armchair seat fully visible and unobstructed;
- tabletop completely clear where required;
- substantial clear section of the windowsill;
- partially empty shelves;
- open floor zones;
- calm wall areas.

**Не использовать прямой процент 60–70 / 30–40 в image prompt.**

Это guideline для дизайнера, но в генераторе может провоцировать artificial filler.

Лучше:

**generous natural breathing space, partially empty shelves, clear floor areas, usable horizontal surfaces**.

---

# 7. Armchair Rule

Если кресло должно быть spawn surface, запрет ставится **сразу в локальном блоке кресла**, а не только в конце prompt.

Использовать жёсткую формулировку:

**The armchair must be shown completely empty. The seat must remain fully visible, unobstructed, and clear for future compositing. No extra pillows, decorative cushions, blanket, throw, book, cup, tray, cat, or objects of any kind. Show only the armchair itself with its built-in upholstery structure.**

Это правило появилось после реального теста: generic `cozy armchair` часто автоматически вызывает pillows/throws.

---

# 8. Prompt Order — Room

Для master room / room iteration использовать порядок:

1. Asset type / exact task
2. Reference usage
3. Camera
4. Room geometry
5. Positional layout
6. Clear/empty game surfaces
7. Permanent details only
8. Style
9. Materials
10. Palette
11. Lighting
12. Critical exclusions

Сначала **layout**, потом **style**.

---

# 9. Prompt Order — Isolated Object

1. Object name
2. Function in game
3. Approved reference if any
4. Exact design / silhouette
5. Surface compatibility
6. View angle
7. Materials
8. Project style
9. Lighting compatibility
10. Background / transparency
11. State: Idle / Loop / Secret
12. Critical exclusions

---

# 10. Animation Consistency

Для animation/edit prompt:

**Preserve exactly:**
- object identity;
- proportions;
- construction;
- texture;
- colors;
- camera;
- orientation;
- scale;
- lighting;
- background/transparency.

**Animate only:** явно перечислить изменяемые компоненты.

Не просить «новую версию» объекта. Просить тот же объект в следующей motion phase.

---

# 11. Seasonal Layer Rule

Seasonal layer — не новая комната.

Формула:

**Additional seasonal decorative layer designed to fit exactly the approved Cozy Sound Room master room without changing architecture, furniture placement, camera, perspective, or permanent object identity.**

Не перемещать мебель. Не менять окно/камин. Не превращать сезон в праздничный maximalism.

---

# 12. Splash Screen Prompt Rules

Состав пресцены генерируется отдельными ассетами:

### Door
- aged natural wooden door;
- believable residential character;
- tactile hand-painted surface;
- no medieval/fantasy architecture;
- front or near-front view.

### Keyhole / Lock
- brass-trimmed keyhole;
- visually clean silhouette;
- large enough to act as interactive focus;
- separate states where needed.

### Game Title
- separate transparent asset;
- exact text: `COZY SOUND ROOM`;
- handcrafted literary lettering;
- subtle Arts & Crafts character;
- no generic fantasy font;
- no heavy gaming-logo styling;
- no neon.

Не просить один image generator одновременно создать дверь и идеально написать финальный логотип, если требуется чистый production-ready title asset.

---

# 13. Negative Prompt System

Не использовать огромный список запретов.

Оставлять 5–8 **самых опасных** ошибок для текущей задачи.

Для master room обычно достаточно:
- no sofa;
- no work desk;
- no extra objects on spawn surfaces;
- no photorealistic photography / glossy CGI;
- no fantasy architecture;
- no excessive clutter;
- no strong baked time-of-day lighting;
- no extreme wide-angle / mathematical isometric.

Локальные запреты важнее универсального negative block.

---

# 14. Do Not Repeat the Same Idea

Если уже сказано `matte tactile natural materials`, не повторять `natural / tactile / matte` ещё в нескольких абзацах.

Повтор допускается только для действительно критичного constraint, который модель системно нарушает, например clear armchair seat.

---

# 15. Prompt Length Modes

## SHORT
Subject + layout + key style + 3–5 constraints.

## STANDARD
Основной формат для простых объектов и слоёв.

## PRODUCTION / STRICT
Точный layout, reference behavior, required clear surfaces, materials, lighting, exclusions, consistency.

Production prompt должен быть **компактнее полной проектной спецификации**. Обычно лучше короткий пространственно конкретный prompt, чем длинный пересказ всех источников.

---

# 16. Universal Generator Prompt Skeleton

**[TASK]**
Create a [specific image/asset].

**[REFERENCE]**
Use the provided reference for [geometry / identity / composition] only. Preserve [critical elements]. Do not copy [unapproved style/materials].

**[CAMERA / LAYOUT]**
[exact viewpoint + positional language].

**[REQUIRED CONTENT]**
[exact objects and placement].

**[CLEAR SURFACES / GAME READABILITY]**
[what must stay empty and readable].

**[STYLE / MATERIALS]**
[compressed Project DNA].

**[COLOR / LIGHT]**
[relevant palette + lighting only].

**[CRITICAL EXCLUSIONS]**
[5–8 relevant negatives].

---

# 17. Preflight Checklist

Перед выдачей prompt проверить:

- [ ] Ясно ли, что именно генерируется?
- [ ] Используется ли reference в правильной роли?
- [ ] Есть ли positional language?
- [ ] Не пересказывается ли лишняя архитектура проекта?
- [ ] Не добавлены ли объекты другого слоя?
- [ ] Свободны ли нужные spawn-поверхности?
- [ ] Для кресла нет ли pillows/throws, если оно должно быть пустым?
- [ ] Не используется ли прямой 60–70% fill target в image prompt?
- [ ] Negative block короткий и релевантный?
- [ ] Не переизобретает ли prompt approved master layout без запроса?
- [ ] Для animation сохранён exact same design?
- [ ] Final prompt визуально исполним, а не просто концептуально правильный?

---

# 18. Приоритет решений

1. Последнее прямое решение пользователя
2. Approved reference image / approved current result
3. Актуальный Handoff
4. Visual Bible
5. Project Specification / Asset Registry
6. Research Report
7. Общие дизайнерские знания

---

# 19. Главная формула v2

**Prompt 2 = spatial skeleton. Prompt 1 = compressed visual DNA.**

Не склеивать два длинных промпта механически.

Prompt Architect должен:
- взять пространственную точность и positional language;
- добавить только релевантный Project DNA;
- убрать невизуальную техническую информацию;
- сохранить clear surfaces;
- выдать более короткий и более исполнимый generator prompt.
