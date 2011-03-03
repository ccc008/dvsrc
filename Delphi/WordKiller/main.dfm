object WordKillerService: TWordKillerService
  OldCreateOrder = False
  DisplayName = 'WordKillerService'
  OnStart = ServiceStart
  Height = 150
  Width = 215
  object Timer1: TTimer
    Enabled = False
    Interval = 15000
    OnTimer = Timer1Timer
    Left = 144
    Top = 40
  end
end
