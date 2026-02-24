# Captain Audio Ver 1.0
*The English description is below

발하임(Valheim) 게임용 커스텀 오디오 모드입니다. 음악, 환경음, 효과음을 DLL에 직접 포함시켜 배포할 수 있습니다.

## 🎵 주요 기능

- **DLL 내장 오디오**: 음악 파일들을 DLL에 EmbeddedResource로 포함
- **음악 교체**: 배경음악, 지역별 음악, 시간대별 음악 교체
- **환경음 교체**: 바이옴별 주변음, 낮/밤 환경음 교체  
- **효과음 교체**: 포털, 횃불, 모닥불 등 오브젝트 소리 교체
- **외부 파일 지원**: 기존 외부 폴더 방식도 여전히 지원

## 📁 폴더 구조

```
Captain_audio/
├── customaudio.cs              # 메인 모드 코드
├── CaptainAudio.csproj         # 프로젝트 파일
├── README.md                   # 이 파일
└── asset/
    └── Resources/
        ├── Music/              # 배경음악 파일들 (.ogg, .wav, .mp3)
        ├── Ambient/            # 환경음 파일들
        └── SFX/                # 효과음 파일들
```

## 🎯 음악 파일 추가 방법

### 1. DLL 내장 방식 (권장)
음악 파일들을 다음 위치에 저장:
- `asset/Resources/Music/` - 일반 배경음악
- `asset/Resources/Music/폴더명/` - 그룹별 음악 (예: Meadows, BlackForest 등)

### 2. 외부 파일 방식 (기존 방식)
- `BepInEx/plugins/CaptainAudio/Music/` 폴더에 음악 파일 저장

## 🔧 빌드 방법

```bash
# VALHEIM_INSTALL 환경변수가 설정되어 있는지 확인
echo $VALHEIM_INSTALL

# 프로젝트 빌드
dotnet build CaptainAudio.csproj -c Release

# 또는 Visual Studio에서 빌드
```

## 📋 지원 파일 형식

- **.ogg** (권장) - Vorbis 압축, 파일 크기 작음
- **.wav** - 무압축, 고품질
- **.mp3** - MPEG 압축 (일부 환경에서 제한적)

## 🎮 게임 내 명령어

콘솔(F5)에서 사용 가능한 명령어:

- `captainaudio music` - 현재 음악 정보 출력
- `captainaudio env` - 현재 환경 정보 출력  
- `captainaudio reset` - 설정 리로드

## ⚙️ 설정 옵션

- **MusicVol**: 음악 볼륨 (기본값: 0.6)
- **AmbientVol**: 환경음 볼륨 (기본값: 0.3)
- **LocationVol**: 지역 음악 볼륨 배수 (기본값: 5.0)
- **DumpInfo**: 디버그 정보 출력 여부 (기본값: false)

## 🎵 음악 매핑 예시

### 기본 음악
- `menu` - 메인 메뉴 음악
- `home` - 집에서의 안전한 음악

### 지역별 시간대 음악 (폴더로 구성)
- `MeadowsMorning/` - 초원 아침 음악들
- `MeadowsDay/` - 초원 낮 음악들  
- `MeadowsEvening/` - 초원 저녁 음악들
- `MeadowsNight/` - 초원 밤 음악들

### 효과음
- `portal` - 포털 소리
- `groundtorch` - 지상 횃불 소리
- `walltorch` - 벽 횃불 소리
- `bonfire` - 모닥불 소리

## 🚀 설치 방법

1. 빌드된 `CaptainAudio.dll`을 `BepInEx/plugins/CaptainAudio/`에 복사
2. 게임 실행 후 F5 콘솔에서 `captainaudio music` 명령어로 확인

## 📝 변경 사항 (Ver 1.0)

- 네임스페이스를 `CaptainAudio`로 변경
- EmbeddedResource 방식으로 DLL에 음악 파일 내장
- 기존 외부 파일 방식과 병행 지원
- 향상된 로깅 및 오류 처리

## English
Built-in Music (Embedded in DLL)
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
| Boss (Total) | 10 |
| - Eikthyr | 1 |
| - The Elder | 2 |
| - Bonemass | 1 |
| - Moder | 1 |
| - Yagluth | 2 |
| - The Queen | 1 |
| - Fader | 2 |

Total: 44 tracks

Custom Music Priority
When you add music files to the custom folder, ONLY your custom music will play!

The built-in music for that biome/location is completely replaced.

#### How to Use

Create folder: BepInEx/plugins/CaptainAudio/Music/{FolderName}/
Add your .ogg, .wav, or .mp3 files
Start game - only your custom music plays for that location

#### Example

BepInEx/plugins/CaptainAudio/
├── CaptainAudio.dll
└── Music/
    └── Meadow/            ← Add 2 custom songs here
        ├── my_song1.ogg
        └── my_song2.ogg


Result: Built-in 3 Meadow tracks are ignored. Only your 2 custom songs play randomly.

#### Folder Names

Use these exact folder names:
Meadow, BlackForest, Swamp, Swamprainday, Mountain, Plain
Mistland, Ashland, Sailing, Home, Menu
Morning, Evening, forestcrypt, frostcaves
Boss_Eikthyr, Boss_TheElder, Boss_Bonemass, Boss_Moder
Boss_Yagluth, Boss_TheQueen, Boss_Fader

## Support / 지원

- Discord: KorCaptainSkillTree_MOD_Server -  https://discord.gg/W26PTxYhug
- E-mail : ssunyme@naver.com
- Issues: Report bugs and suggestions on Discord

---

## Credits

- **Developer**: KorCaptain
