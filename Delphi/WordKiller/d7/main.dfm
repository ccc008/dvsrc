object WordKillerService: TWordKillerService
  OldCreateOrder = False
  DisplayName = 'WordKillerService'
  OnStart = ServiceStart
  Height = 150
  Width = 215
  object TimerTestKillByWindow: TTimer
    Enabled = False
    Interval = 15000
    OnTimer = TimerTestKillByWindowTimer
    Left = 144
    Top = 40
  end
  object TimerOutdatedProcesses: TTimer
    Enabled = False
    Interval = 15000
    OnTimer = TimerOutdatedProcessesTimer
    Left = 16
    Top = 16
  end
end
