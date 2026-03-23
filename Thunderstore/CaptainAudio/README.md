# Captain Audio

[![Ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/korcaptain)

**Replace Valheim's music, ambient sounds, and sound effects with custom audio files embedded directly in the mod DLL.**

Transform your Valheim experience with 64MB of custom music covering all biomes, bosses, time-based themes, and more. Captain Audio provides a complete audio replacement system with real-time controls and easy customization.

---

<details>
<summary>📖 About This Mod</summary>

Captain Audio is an **enhanced and extended version** of [aedenthornCustomAudio](https://thunderstore.io/c/valheim/p/TRGC/aedenthornCustomAudio/), adding:
- **44 high-quality music tracks** embedded directly in the DLL (64MB total)
- Pre-configured music for all biomes, bosses, and activities
- **Ready to use out-of-the-box** - no additional setup required
- Full compatibility with the original CustomAudio external file system

This mod builds upon aedenthornCustomAudio's flexible audio replacement framework while providing a complete curated soundtrack experience.

</details>

---

<details>
<summary>✨ Features</summary>

### 🎵 Music Replacement
- **44 embedded audio files** covering:
  - 🐗 Boss battles (Eikthyr, Elder, Bonemass, Moder, Yagluth, The Queen)
  - 🌲 All biomes (Meadows, Black Forest, Swamp, Mountains, Plains, Mistlands, Ashlands)
  - 🌅 Time-based themes (Morning, Evening)
  - 🏠 Activities (Home, Sailing, Menu)
  - 🏰 Dungeons (Forest Crypt, Frost Caves)

### 🌊 Ambient Sound Replacement
- Environment sounds for each biome
- Ocean and wind sounds with special handling
- Day/night ambient variations

### 🔊 Sound Effects (SFX) Replacement
- Portal sounds
- Fireplace/torch sounds (5 types: ground torch, wall torch, bonfire, etc.)
- Customizable object sounds

### ⚙️ Real-time Controls
- **Console commands** for instant audio control
- **Config file** for persistent settings
- **Volume controls** that apply immediately
- **Reset functionality** to restore vanilla audio

</details>

---

<details>
<summary>🎵 Included Music (44 Tracks)</summary>

### Biomes (23 tracks)
- **Meadows**: 3 tracks
- **Black Forest**: 3 tracks
- **Swamp**: 3 tracks + 1 rainy day variant
- **Mountains**: 3 tracks
- **Plains**: 3 tracks
- **Mistlands**: 3 tracks
- **Ashlands**: 2 tracks
- **Forest Crypt** (dungeon): 2 tracks
- **Frost Caves** (dungeon): 1 track

### Boss Battles (10 tracks)
- **Eikthyr**: 1 track
- **The Elder**: 2 tracks
- **Bonemass**: 1 track
- **Moder**: 1 track
- **Yagluth**: 2 tracks
- **The Queen**: 1 track

### Time & Activities (11 tracks)
- **Morning**: 2 tracks
- **Evening**: 1 track
- **Home**: 3 tracks
- **Sailing**: 2 tracks
- **Menu**: 1 track

</details>

---

<details>
<summary>📦 Installation</summary>

### Method 1: r2modman (Recommended)

1. **Install r2modman** if you haven't already: [r2modman Download](https://valheim.thunderstore.io/package/ebkr/r2modman/)
2. **Select Valheim** as your game
3. **Search for "CaptainAudio"** in the mod browser
4. **Click "Download"** and let r2modman handle everything
5. **Launch the game** through r2modman

### Method 2: Manual Installation

1. **Install BepInEx** if not already installed: [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. **Download CaptainAudio.dll** from this mod page
3. **Navigate to** `BepInEx/plugins/` folder
4. **Create folder** `CaptainAudio` (if it doesn't exist)
5. **Copy CaptainAudio.dll** into the folder
6. **Launch Valheim**

**Installation Path:**
```
Valheim/
└── BepInEx/
    └── plugins/
        └── korCaptain-CaptainAudio/
              └── CaptainAudio.dll
```

</details>

---

<details>
<summary>🎮 Usage</summary>

### Console Commands

Press **F5** in-game to open the console, then use these commands:

| Command | Description | Example Output |
|---------|-------------|----------------|
| `captainaudio music` | Display current music info | Shows playing track, biome, time of day |
| `captainaudio env` | Display environment audio info | Shows ambient sounds, ocean/wind settings |
| `captainaudio reset` | Reload settings from config | Reapplies all config values |

**Example:**
```
F5 (open console)
> captainaudio music
Current Music: Meadows_Morning_01.ogg
Biome: Meadows
Time: Morning
Volume: 0.6
```

### Configuration File

After first launch, edit the config file at:
```
BepInEx/config/captain.CaptainAudio.cfg
```

| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| `Enabled` | `true` | true/false | Enable/disable the mod |
| `MusicVolume` | `0.6` | 0.0 - 1.0 | Background music volume |
| `AmbientVolume` | `0.3` | 0.0 - 1.0 | Environment sounds volume |
| `LocationVolumeMultiplier` | `5.0` | 0.0 - 10.0 | Volume boost for location-specific music |

**Example Config:**
```ini
[General]
Enabled = true
MusicVolume = 0.6
AmbientVolume = 0.3
LocationVolumeMultiplier = 5.0

[Advanced]
LoadRetryCount = 3
LoadRetryDelay = 0.5
LoadTimeout = 15
EnableFallback = true
```

> Config changes apply immediately in-game. Use `captainaudio reset` to reload if needed.

</details>

---

<details>
<summary>🎨 Music Customization</summary>

### Adding Your Own Music

**Method 1: External Files** (✅ Recommended)
1. Navigate to: `BepInEx/plugins/korCaptain-CaptainAudio/`
2. Create folders:
   ```
   BepInEx/plugins/korCaptain-CaptainAudio/
   ├── Music/       # Your custom music files
   ├── Ambient/     # Custom ambient sounds
   └── SFX/         # Custom sound effects
   ```
3. Add your audio files (`.ogg`, `.wav`, `.mp3`)
4. **Restart Valheim** to load new music

> Custom music **overrides embedded tracks** for that biome/boss.

### Folder Names

Use these exact folder names:
- **Biomes**: `Meadows`, `BlackForest`, `Swamp`, `Swamprainday`, `Mountain`, `Plain`, `Mistland`, `Ashland`
- **Bosses**: `Boss_Eikthyr`, `Boss_TheElder`, `Boss_Bonemass`, `Boss_Moder`, `Boss_Yagluth`, `Boss_TheQueen`, `Boss_Fader`
- **Activities**: `Morning`, `Evening`, `Home`, `Sailing`, `Menu`
- **Dungeons**: `forestcrypt`, `frostcaves`

**Multiple files per folder = random selection each time.**

### Example Structure
```
BepInEx/plugins/korCaptain-CaptainAudio/Music/
├── Meadows/
│   ├── peaceful1.ogg
│   └── peaceful2.ogg   # 2 tracks → random
├── Boss_Eikthyr/
│   └── epic_battle.ogg
└── Mountain/
    └── mountain_theme.ogg
```

### Supported Audio Formats
- `.ogg` - **Recommended** (best compatibility, smaller size)
- `.wav` - Uncompressed (high quality, larger files)
- `.mp3` - MPEG compression (limited support)

</details>

---

<details>
<summary>🔧 Troubleshooting</summary>

### Music Not Playing
1. Check console (F5) for error messages
2. Verify BepInEx is installed correctly
3. Try `captainaudio reset` command
4. Check config file: `Enabled = true`, `MusicVolume > 0.0`

### Volume Too Low/High
- Adjust `MusicVolume` and `AmbientVolume` in config file
- Use `captainaudio reset` to apply changes
- For location music, adjust `LocationVolumeMultiplier`

### Audio Cuts Out or Stutters
- Check `LoadRetryCount` and `LoadTimeout` in Advanced config
- Ensure sufficient disk space
- Try reducing `LocationVolumeMultiplier` if location music overlaps

### Config File Not Found
- Launch the game once to generate the config file
- Location: `BepInEx/config/captain.CaptainAudio.cfg`
- Default values will be created automatically

</details>

---

<details>
<summary>📋 Technical Details</summary>

- **Framework:** .NET Standard 2.1
- **BepInEx Version:** 5.4.2200+
- **Total Audio:** 64MB embedded (44 files)
- **Audio Formats:** OGG (primary), MP3, WAV
- **Harmony Patches:** 10 patches for comprehensive audio replacement
- **Performance:** Parallel loading, reflection caching, retry logic

</details>

---

<details>
<summary>🌐 Compatibility</summary>

- ✅ **Singleplayer:** Fully supported
- ✅ **Multiplayer:** Client-side only (other players won't hear your music)
- ✅ **Dedicated Servers:** No server-side installation needed
- ✅ **Other Mods:** Compatible with most mods (no known conflicts)

</details>

---

## 🇰🇷 한국어

**발헤임의 모든 음악을 64MB 고품질 커스텀 사운드트랙으로 교체합니다.**

---

<details>
<summary>🎵 내장 음악 목록 (44곡)</summary>

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

</details>

---

<details>
<summary>🎨 커스텀 음악 사용법</summary>

커스텀 폴더에 음악을 넣으면 **내장 음악을 완전히 대체**합니다.

#### 사용 방법

1. 폴더 생성: `BepInEx/plugins/korCaptain-CaptainAudio/Music/{폴더명}/`
2. `.ogg`, `.wav`, `.mp3` 파일 추가
3. 게임 재시작 - 해당 지역에서 커스텀 음악만 재생

#### 폴더명
- `Meadow`, `BlackForest`, `Swamp`, `Swamprainday`, `Mountain`, `Plain`
- `Mistland`, `Ashland`, `Sailing`, `Home`, `Menu`
- `Morning`, `Evening`, `forestcrypt`, `frostcaves`
- `Boss_Eikthyr`, `Boss_TheElder`, `Boss_Bonemass`, `Boss_Moder`
- `Boss_Yagluth`, `Boss_TheQueen`, `Boss_Fader`

#### 예시
```
BepInEx/plugins/CaptainAudio/
├── CaptainAudio.dll
└── Music/
    └── Meadow/
        ├── my_song1.ogg
        └── my_song2.ogg   ← 랜덤 재생
```

> 같은 폴더에 여러 곡을 넣으면 게임이 랜덤으로 선택합니다.

</details>

---

## 🎮 My Other Mods / 함께 즐기면 더 좋은 모드

### ⚔️ [CaptainSkillTree](https://thunderstore.io/c/valheim/p/KorCaptain/CaptainSkillTree/)

> 🎵 **CaptainAudio와 함께 설치하면 스킬 발동 시 웅장한 음악이 울려퍼지며 전투 몰입감이 극대화됩니다!**
>
> Valheim에 완전한 스킬트리 시스템 추가 — 전문가 트리, 무기 트리, 7개 직업 시스템으로 바이킹을 강화하세요. 두 모드를 함께 설치하면 **재미가 100% 증가**합니다!
>
> 🎵 **Install alongside CaptainAudio for epic music on skill activation — maximum battle immersion!**
>
> Adds a full skill tree system to Valheim — Expert Trees, Weapon Trees, and 7 Job Classes. **100% more fun** when used together!

---

## 📝 Credits

- **Mod Author:** [korCaptain]
- **Based on:** [aedenthornCustomAudio](https://thunderstore.io/c/valheim/p/TRGC/aedenthornCustomAudio/) by TRGC
- **BepInEx:** BepInEx team
- **Harmony:** pardeike

---

## 🤝 Support / 지원

- **Discord:** https://discord.gg/W26PTxYhug
- **Ko-fi:** https://ko-fi.com/korcaptain
- **Email:** ssunyme@naver.com
- **GitHub:** https://github.com/KorCaptain/Valheim_Captain_Audio

---

**Enjoy your personalized Valheim soundtrack! 🎵 / 당신만의 발헤임 사운드트랙을 즐기세요!**
