   M S B A M L   `   `   `  ����J  FTeleprompter, Version=0.5.21.180, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Teleprompter.Wpf_ControlsTeleprompter.Wpf_Controls  5      &     Teleprompter.Wpf_Controls.Editor   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      5w'clr-namespace:Teleprompter.Wpf_Controls     5      -   5        .+�5
      �� -   5        $��	rtfEditor��5
   $   $��0,0,0,0q�6   "  �� VerticalScrollBarVisibility$  Visible=�65   $��Stretch=�5      $��Stretch=�6/   
���   �
�������$��25(�6s   $��Arial)�6�   $��0q�5   �   
���   � � 	Resources 5   
    �� 
TargetType �� Property �� Value��%�   'J�       �� 5      " J�6   .�5      ��  �@��Margin$ 0q�6+   �� 5       �@��LineStackingStrategy$ BlockLineHeight=�69   5      5       � ContextMenu 5   
   �� 5      .@�5      m�  m� Command$	 Cut��6   m� 5      $
 Copy��6   m� 5      $ Paste��6   m� 5      $ w:Command.AddMarker��6   m� 5      $ w:Command.AddSlideMarker��6   m� 5      $ w:Command.AddCameraMarker��6   5       J� InputBindings 5   
   �� 5       �� Command$ w:Command.ColorWhite��6   	 �� Gesture$	 CTRL+1��68   �� 5      $ w:Command.ColorYellow��6   $	 CTRL+2��6:   �� 5       $ w:Command.ColorGreen��6   $	 CTRL+3��69   �� 5!      $ w:Command.ColorRed��6   $	 CTRL+4��67   �� 5"      $ w:Command.ColorBlue��6   $	 CTRL+5��68   �� 5#      $ w:Command.AllCaps��6   $	 CTRL+6��66   �� 5$      $ w:Command.SentenceCaps��6   $	 CTRL+7��6;   �� 5%      $ w:Command.AddMarker��6   $	 CTRL+M��68   �� 5&      $ w:Command.AddSlideMarker��6   $	 CTRL+SHIFT+M��6=   �� 5'      $ w:Command.AddCameraMarker��6   $	 CTRL+SHIFT+C��6>   �� 5(      $ w:Command.PrevMarker��6   $	 CTRL+UP��69   �� 5)      $ w:Command.NextMarker��6   $	 	CTRL+DOWN��69   �� 5*      $ w:Command.Escape��6   $		 ESC��65   �� 5+      $ w:Command.ToggleScroll��6   $	 F2��6;   �� 5,      $ w:Command.AdjustFontDown��6   $	 	CTRL+LEFT��6=   �� 5-      $ w:Command.AdjustFontUp��6   $	 
CTRL+RIGHT��6;   5/      �� 50      -   5        $��rtfTemp��50   $   $��0,0,0,0q�6   $��Stretch=�63   $��Stretch=�6O   
���   �
�������$��0q�6�   
���   �$y�Hidden=�6�   . �51   
   /� .T�52      J� .*�6   TEST54      55      56      
 J� CommandBindings
 58      )  #System.Windows.Input.CommandBinding  59   
   -   5           Command$ w:Command.AddMarker��59      67     5:   
   -   5        $ w:Command.AddSlideMarker��5:      6<     5;   
   -   5        $ w:Command.AddCameraMarker��5;      6=     5<   
   -   5        $ w:Command.ColorWhite��5<      68     5=   
   -   5        $ w:Command.ColorYellow��5=      69     5>   
   -	   5        $ w:Command.ColorGreen��5>      68     5?   
   -
   5        $ w:Command.ColorRed��5?      66     5@   
   -   5        $ w:Command.ColorBlue��5@      67     5A   
   -   5        $ w:Command.AllCaps��5A      65     5B   
   -   5        $ w:Command.SentenceCaps��5B      6:     5C   
   -   5        $ w:Command.PrevMarker��5C      68     5D   
   -   5        $ w:Command.NextMarker��5D      68     5E   
   -   5        $ w:Command.Escape��5E      64     5F   
   -   5        $ w:Command.ToggleScroll��5F      6:     5G   
   -   5        $ w:Command.AdjustFontDown��5G      6<     5H   
   -   5        $ w:Command.AdjustFontUp��5H      6:   5J      