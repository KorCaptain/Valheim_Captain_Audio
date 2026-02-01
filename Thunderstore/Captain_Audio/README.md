# Captain Audio

Custom audio replacement mod for Valheim.

---

## English

### Built-in Music (Embedded in DLL)

| Biome/Location | Tracks |
|----------------|--------|
| Meadow | 3 |
| Black Forest | 3 |
| Swamp | 3 |
| Swamp Rain Day | 1 |
| Mountain | 3 |
| Plain | 3 |
| Mistland | 3 |
| Ashland | 2 |
| Sailing | 3 |
| Home | 3 |
| Menu | 1 |
| Morning | 2 |
| Evening | 1 |
| Forest Crypt | 2 |
| Frost Caves | 1 |
| **Boss (Total)** | **10** |
| - Eikthyr | 1 |
| - The Elder | 2 |
| - Bonemass | 1 |
| - Moder | 1 |
| - Yagluth | 2 |
| - The Queen | 1 |
| - Fader | 2 |

**Total: 44 tracks**

### Custom Music Priority

**When you add music files to the custom folder, ONLY your custom music will play!**

The built-in music for that biome/location is completely replaced.

#### How to Use

1. Create folder: `BepInEx/plugins/CaptainAudio/Music/{FolderName}/`
2. Add your `.ogg`, `.wav`, or `.mp3` files
3. Start game - only your custom music plays for that location

#### Example

```
BepInEx/plugins/CaptainAudio/
├── CaptainAudio.dll
└── Music/
    └── Meadow/            ← Add 2 custom songs here
        ├── my_song1.ogg
        └── my_song2.ogg
```

**Result**: Built-in 3 Meadow tracks are ignored. Only your 2 custom songs play randomly.

#### Folder Names

Use these exact folder names:
- `Meadow`, `BlackForest`, `Swamp`, `Swamprainday`, `Mountain`, `Plain`
- `Mistland`, `Ashland`, `Sailing`, `Home`, `Menu`
- `Morning`, `Evening`, `forestcrypt`, `frostcaves`
- `Boss_Eikthyr`, `Boss_TheElder`, `Boss_Bonemass`, `Boss_Moder`
- `Boss_Yagluth`, `Boss_TheQueen`, `Boss_Fader`

---

## 한국어

### 내장 음악 (DLL에 포함)

| 바이옴/지역 | 곡 수 |
|------------|-------|
| 초원 (Meadow) | 3곡 |
| 검은숲 (BlackForest) | 3곡 |
| 늪지대 (Swamp) | 3곡 |
| 늪지대 비오는날 (Swamprainday) | 1곡 |
| 산악 (Mountain) | 3곡 |
| 평원 (Plain) | 3곡 |
| 안개땅 (Mistland) | 3곡 |
| 잿빛땅 (Ashland) | 2곡 |
| 항해 (Sailing) | 3곡 |
| 집 (Home) | 3곡 |
| 메뉴 (Menu) | 1곡 |
| 아침 (Morning) | 2곡 |
| 저녁 (Evening) | 1곡 |
| 숲 던전 (forestcrypt) | 2곡 |
| 얼음 동굴 (frostcaves) | 1곡 |
| **보스 (총합)** | **10곡** |
| - 에이크튀르 (Eikthyr) | 1곡 |
| - 장로 (TheElder) | 2곡 |
| - 본매스 (Bonemass) | 1곡 |
| - 모데르 (Moder) | 1곡 |
| - 야글루스 (Yagluth) | 2곡 |
| - 여왕 (TheQueen) | 1곡 |
| - 페이더 (Fader) | 2곡 |

**총합: 44곡**

### 커스텀 음악 우선순위

**커스텀 폴더에 음악을 넣으면, 해당 바이옴은 커스텀 음악만 재생됩니다!**

내장 음악은 완전히 무시됩니다.

#### 사용 방법

1. 폴더 생성: `BepInEx/plugins/CaptainAudio/Music/{폴더명}/`
2. `.ogg`, `.wav`, `.mp3` 파일 추가
3. 게임 시작 - 해당 지역에서 커스텀 음악만 재생

#### 예시

```
BepInEx/plugins/CaptainAudio/
├── CaptainAudio.dll
└── Music/
    └── Meadow/            ← 여기에 커스텀 음악 2곡 추가
        ├── my_song1.ogg
        └── my_song2.ogg
```

**결과**: 내장된 초원 음악 3곡은 무시되고, 커스텀 음악 2곡만 랜덤 재생됩니다.

#### 폴더명

아래 폴더명을 정확히 사용하세요:
- `Meadow`, `BlackForest`, `Swamp`, `Swamprainday`, `Mountain`, `Plain`
- `Mistland`, `Ashland`, `Sailing`, `Home`, `Menu`
- `Morning`, `Evening`, `forestcrypt`, `frostcaves`
- `Boss_Eikthyr`, `Boss_TheElder`, `Boss_Bonemass`, `Boss_Moder`
- `Boss_Yagluth`, `Boss_TheQueen`, `Boss_Fader`

---

## Support

- GitHub: https://github.com/KorCaptain/Valheim_Captain_Audio
- Issues: https://github.com/KorCaptain/Valheim_Captain_Audio/issues
