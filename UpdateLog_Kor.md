# 개발 로그
> [English Log](./UpdateLog.md)

### 2024/3/18
- 리포지토리 생성

### 2024/3/28 - v0.0.1
- 프로토타입 버전 개발

  <details>
    <summary>최초 기능</summary>
    
    1. 좌클릭으로 UI 오브젝트 선택
    2. 선택된 오브젝트 드래그
    3. 선택된 오브젝트 하이라이트
    4. 노드 클릭&드래그 : 캔버스에서 선 렌더링

    ![1-1](https://github.com/ysj0828/NodeSystem/assets/63217600/312a31f4-ec84-4ba0-ba36-cd841bd5aed0)


  </details>
  
  <details>
    <summary>버그</summary>
    
    1. 렌더링된 선이 엉뚱한 위치에서 렌더링
 
    ![2-1](https://github.com/ysj0828/NodeSystem/assets/63217600/02615dc2-c808-47b0-9db6-a605ba5c48af)
      
  </details>

### 2024/4/1 - v0.0.2
- 이전 오류 수정

  <details>
    <summary>업데이트</summary>
    
    1. 버그 수정
        - 렌더링된 선이 엉뚱한 위치에서 렌더링

  </details>
  
  <details>
    <summary>버그</summary>
    
    1. 연결된 선이 렌더링 안되는 이슈
    2. 노드에서 드래그중일 때 선이 주변 노드에 자동으로 스냅되지 않는 이슈

    ![3-1](https://github.com/ysj0828/NodeSystem/assets/63217600/ba55d38d-23b7-413f-897e-a05b8ce29f52)

  </details>


### 2024/4/16 - v0.0.3
- 이전 오류 수정

  <details>
    <summary>업데이트</summary>
    
    1. Bug fix
        - 연결된 선이 렌더링 안되는 이슈

  </details>
  
  <details>
    <summary>버그</summary>
    
    1. 노드에서 드래그중일 때 선이 주변 노드에 자동으로 스냅되지 않는 이슈

    ![4-1](https://github.com/ysj0828/NodeSystem/assets/63217600/123649d7-73c0-4cc9-9e37-3a9785616c78)


  </details>


### 2024/4/30 - v0.0.4
- 기능 추가 : 근처 노드에 스냅

  <details>
    <summary>업데이트</summary>
    
    1. 기능 : 근처 노드에 스냅
         - 연결 선을 드래그 중일 때 근처에 노드가 있으면 자동으로 스냅
    
    ![5-1](https://github.com/ysj0828/NodeSystem/assets/63217600/e80a0f24-4f98-4a33-8813-9d4d284ab1fa)

  </details>


### 2024/5/15 - v0.1
- 기능 추가 : 연결 끊기

  <details>
    <summary>업데이트</summary>
    
    1. 기능 : 연결 끊기
         - 연결 선을 빈 공간으로 드래그하여 연결을 끊는 기능
    
    ![6-1](https://github.com/ysj0828/NodeSystem/assets/63217600/7f8febfd-e6a7-4f00-880e-998d57a3af88)


  </details>


### 2024/6/5 - v0.1.1
- 기능 업데이트 : 노드 연결

  <details>
    <summary>Update</summary>
    
    1. 기능 : 노드 연결
         - 아웃풋 노드에서만 연결을 시작할 수 있게끔 변경

  </details>
