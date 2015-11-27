## kgc 2015에서 강연한 'C# 스크립트를 사용한 게임서버 모니터링 시스템 개발'의 Demo 코드


## 구현된 기능
- 서버 애플리케이션 on/off
- 서버 머신 및 서버 애플리케이션 CPU, RAM 사용량 표시
- 서버 애플리케이션 리소스 업데이트. AWS S3 이용.
- 관리자 로그인
 

## 구현 예정 기능
- (TCP 혹은 http 사용)서버 애플리케이션가 좀비가 되었는지 조사(Agent와 애플리케이션 서버간의 네트워크 통신)
- 아직 미 실행된 관리자 요청 보여주기, 요청이 오래된 것은 자동 취소(5초)
- 로그 남기기(파일로)
- 클라이언트 UI에 Visualization: Gauge 적용
    - https://developers.google.com/chart/interactive/docs/gallery/gauge
