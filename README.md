## kgc 2015에서 강연한 'C# 스크립트를 사용한 게임서버 모니터링 시스템 개발'의 Demo 코드


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


## 구성
- 저장소의 MonitoringSystem 디렉토리에 이름 그대로 모니터링 시스템의 Server와 Agent가 있다.
    - Server는 클라이언트(관리자 혹은 운영자)가 접속하여 복수의 '서버 애플리케이션(게임서버,로그인 서버 등을 가르킨다)'의 상태를 파악하거나 특정 Agent에 명령을 내릴 수 있다.    
    - Agent는 '서버 애플리케이션(게임서버,로그인 서버 등을 가르킨다)'이 설치되는 컴퓨터에 복사한다.
    - Agent는 주기적으로 http 통신으로 Server와 통신한다.
- 저장소의 ServerApps는 테스트용으로 사용되는 '서버 애플리케이션'이 있다.
    - 'HttpGameServer'는 WCF로 만들어진 서버이며, 빌드는 VS 2013 이상이 필요하고, 빌드 후 실행 파일은 Bin 디렉토리에 생성되고, 실행에는 관리자 권한이 필요하다.


## 실행 하기
- ScriptCS를 설치해야 한다.
- Server나 Agent를 실행하지 않고 특정 기능을 테스트 해보고 싶을 때는 Agent 디렉토리에 있는 unitTest.csx을 참고해서 코드 테스트를 해볼 수 있다.
- Server, Agent 실행 순서는 상관 없다.

### Server
- 'server.csx'가 주 실행 부이다. 아래의 명령어로 실행한다.
    - scriptcs server.csx
    - server.csx 파일을 열어 보면 상단에 로그를 출력하는 실행 명령어가 주석으로 적어 있다.
    - Server의 port 번호는 8888 이다. 만약 port 번호를 바꾸고 싶다면 https://github.com/adamralph/scriptcs-nancy 를 참조하기 바란다. 참고로 Server의 웹프레임워크는 Nancy를 사용하였다.

### Agent
- configuration.csx 파일을 열어서 Server의 http 주소, Agent가 실행되는 컴퓨터의 IP, Agent가 관리하는 서버 애플리케이션의 정보를 수정해야 한다.
- AWS의 S3를 사용하려면 aws_s3_service.csx 파일을 열어서 aws의 접근키, 패스워드, S3의 버킷 이름을 입력해야 한다.
- 위의 두 파일을 수정을 한 후 Agent를 실행한다.
- 'agent.csx'가 주 실행 부이다. 아래의 명령어로 실행한다.
    - scriptcs agent.csx
    - agent.csx 파일을 열어 보면 상단에 로그를 출력하는 실행 명령어가 주석으로 적어 있다.
