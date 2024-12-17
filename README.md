# Real-time Adaptive AR UI

>사
>사용자는 원하는 스타일을 선택하여 골라 추천받을 수 있습니다.
---
## Index
  - [Motivation](#Motivation)
  - [Development Environment](#Development-Environment)
  - [Guide](#Key-features)
  - [Requirements](#Requirements)
  - [Demo Video](#Demo-Video)

## Motivation

> Image Tracking 기법을 사용한 증강현실 구현을 목표로 Unity, MRTK, Vuforia를 사용
> 
> 멀티 플랫폼(Android, IOS, HoloLens, Windows)에 빌드 가능한 AR UI
>
> 실시간 객체 인식, 주변 조도에 적응 가능한 적응형 UI

## Development Environment

> Vuforia Engine
> 
> Unity 2022.3.54 LTS intel
> 
> MRTK - Mixed Reality Toolkit

## Guide

- Remove background
>Remove background API를 사용하여 사용자가 업로드한 이미지를 딥러닝 모델이 학습한 데이터에 맞추기 위해 배경을 삭제해주는 과정이 들어갑니다.
![Rembg](https://i.imgur.com/ChD28Lw.png)

- Item classifier
>사용자가 업로드한 이미지에 해당하는 상품이 무엇인지 판단합니다. efficientnet_b0모델을 사용하였고 라벨링은 옷 상품에 따라 총 21개입니다.
<div style="display:flex; justify-content:space-between;">
    <img src="https://i.imgur.com/MnwCpVJ.jpg" alt="Model" width="45%">
    <img src="https://i.imgur.com/3ep33HL.jpg" alt="Model" width="45%">
</div>

- Color classifier
>사용자가 업로드한 상품의 이미지가 어떤 색상인지 판단합니다. K-means clustering을 통해 색상 값을 추출하고 사전에 지정한 216가지 색상 값 중 가장 가까운 값을 매핑합니다.
![Color](https://i.imgur.com/RJaKWFi.png)

- Login / Sign up
>사용자 기반 서비스이므로 회원가입과 로그인 서비스를 통해 개인화된 서비스를 제공합니다.
<div style="display:flex; justify-content:space-between;">
    <img src="https://i.imgur.com/lrlGt3z.png" alt="Login" width="45%">
    <img src="https://i.imgur.com/Wo256S3.png" alt="Login" width="45%">
</div>

- Result
>AI 모델이 상품을 판단하고 해당 상품에 어울리는 옷 코디와 가격, 구매 링크 등 세부 정보까지 알려줍니다.
<div style="display:flex; justify-content:space-between;">
    <img src="https://i.imgur.com/I7WyHG5.png" alt="Login" style="width: 45%; object-fit: cover;">
    <img src="https://i.imgur.com/k5KQmo5.png" alt="Login" style="width: 45%; object-fit: cover;">
</div>

## Requirements

프로젝트를 실행하려면 먼저 필요한 라이브러리를 설치해야 합니다. 아래는 필요한 라이브러리들이 나열되어 있는 `requirements.txt` 파일을 사용하여 설치하는 방법입니다.
```sh
pip install -r requirements.txt
```

## Demo Video

YouTube link
https://youtu.be/ONddK9AoYs4?feature=shared

PDF 자료
https://drive.google.com/file/d/1YLiWO9QMu75aqA1TJA5ot12cw2WxyeIl/view?usp=sharing

