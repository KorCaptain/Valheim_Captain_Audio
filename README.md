# Captain Audio Ver 1.0

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