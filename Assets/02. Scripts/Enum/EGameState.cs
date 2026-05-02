public enum EGameState
{
    Tutorial, // 튜토리얼
    RoundReady, // 시작까지 준비 시간
    RoundStart, // 장사 시작
    RoundEnd, // 장사 종료
    RoundPrepare, // 장사 준비
    Pause, // 일시정지
    Resume, // 재개 (이전 상태로)
    RoundRestart, // 라운드 재시작
    Leave, // 로비로
    Quit, // 게임 종료
}