   M S B A M L   `   `   `  ����  AthenaConsole    AthenaConsole.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Athena Console��5   	   $��468.728��6    $��782.244��61    � 
ResizeMode$ NoResize=�6A   .��5      � -   5        $��gridMain��5      .+�5   
   o� $
��Left=�6   $��23��6*   $	��Top=�66   $	��774��6N   .@�5      m� $
��Menu��6   .@�5      m� -   5        $��Option��5      $
��Left=�6H   $	��145��6c   m� 5	      -   5        $
��Exit��5	      $
��Left=�6C   $	��145��6^   5
      5      �� 5   
   $
��Left=�6   $	��416��60   $��0,23,0,0q�6=   $	��Top=�6O   $	��774��6g   .@�5      �� -   5        $��	tAdminTab��5      $��Admin��6'   .��5      � ���	#FFE5E5E5$��	-3,1,3,-1q�6.   .+�5      � $��458,59,22,118q�6   .+�5      �� $��Send Text(s)��6    $
��Left=�67   $��10,9,0,0q�6R   $	��Top=�6d   $��88��6|   $��14(�6�   	� 5      -   5         	� 	GroupName$ 
grpSendTxt��5   B   $��Single��6Y    d� 	IsChecked$
 TrueY�6j   $
��Left=�6{   $��
110,16,0,0q�6�   $	��Top=�6�   	� 5      -   5        $ 
grpSendTxt��5   A   $��Group��6X   $
��Left=�6h   $��
174,16,0,0q�6�   $	��Top=�6�   � 5      -   5        $��
gSingleTxt��5      $
��Left=�61   $	��171��6L   $��	10,28,0,0q�6Y   $	��Top=�6l   $	��272��6�   .+�5      �� -   5        $��tbPhoneNumber��5   �   $
��Left=�6�   $��23��6�   $��	61,11,0,0q�6�    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��135��6  ��6&   ��  �� Path$ ItemName� �� UpdateSourceTrigger$ PropertyChanged=�6  �� 5      -	   5        $��tbSingleMessage��5   &   $
��Left=�6=   $��69��6X   $��8,58,0,0q�6d   $
 Wrap=�6v   $	��Top=�6�   $	��254��6�   �� 5      $��Phone #��6$   $
��Left=�66   $��8,8,0,0q�6Q   $	��Top=�6b   �� 5      -
   5        $��bSingleSend��5   %   $
��Send��6X   $
��Left=�6g   $��
91,129,0,0q�6�   $	��Top=�6�   $��75��6�   �� 5      $��Message��6$   $
��Left=�66   $��8,30,0,0q�6Q   $	��Top=�6c   5      � 5      -   5        $��	gGroupTxt��5      $y�Hidden=�60   $
��Left=�6D   $	��160��6_   $��	10,28,0,0q�6l   $	��Top=�6   $	��274��6�   .+�5      �� -   5        $��cbGroupList��5   '   $
��Left=�6:   $��70,6,0,0q�6U   $	��Top=�6g   $	��120��6   �� 5      -   5        $��tbGroupMessage��5   &   $
��Left=�6<   $��69��6W   $��8,58,0,0q�6c   $
 Wrap=�6u   $	��Top=�6�   $	��254��6�   �� 5      -   5        $��
bGroupSend��5   %   $
��Send��6V   $
��Left=�6e   $��
91,129,0,0q�6�   $	��Top=�6�   $��75��6�   �� 5      $��Group��6$   $
��Left=�64   $��8,8,0,0q�6O   $	��Top=�6`   �� 5       $��Message��6$   $
��Left=�66   $��8,30,0,0q�6Q   $	��Top=�6c   5!      5"      � 5#      $��10,64,345,110q�6   .+�5$      �� $��	All Users��6    $
��Left=�64   $��26,4,0,0q�6O   $	��Top=�6a   $��14(�6y   �� 5%      -   5        $��
lbAllUsers��5%   "   $
��Left=�65   $	��150��6P   $��	-2,27,0,0q�6]   $	��Top=�6p   $	��120��6�   �� 5&      $��Connections��6    $
��Left=�66   $��	150,4,0,0q�6Q   $	��Top=�6d   $��14(�6|   �� 5'      -   5        $��lbOnlineUsers��5'   "   $
��Left=�67   $	��150��6R   $��
132,27,0,0q�6_   $	��Top=�6s   $	��120��6�   �� 5(      -   5        $��bAddUser��5(   !   $	��Add��6J   $
��Left=�6X   $��	5,183,0,0q�6s   $	��Top=�6�   $��44��6�   �� 5)      -   5        $
��Edit��5)   !   $
��Left=�6J   $��
65,183,0,0q�6e   $	��Top=�6y   $��44��6�   �� 5*      -   5        $
��Kick��5*   !   $
��Left=�6J   $��169,183,0,0q�6e   $	��Top=�6z   $��44��6�   �� 5+      -   5        $��
lbContacts��5+   "   $
��Left=�64   $	��150��6O   $��
270,27,0,0q�6\   $	��Top=�6p   $	��120��6�   �� 5,      $��Contacts��6    $
��Left=�63   $��	295,4,0,0q�6N   $	��Top=�6a   $��14(�6y   �� 5-      -   5        $	��Add��5-   !   $
��Left=�6K   $��275,183,0,0q�6f   $	��Top=�6{   $��50��6�   �� 5.      -   5        $
��Edit��5.   !   $
��Left=�6M   $��332,183,0,0q�6h   $	��Top=�6}   $��50��6�   50      � 51      $��280,295,364,12q�6   .+�52      �� $��Groups��6    $
��Left=�61   $��	34,-8,0,0q�6L   $	��Top=�6_   $��14(�6w   �� 53      -   5        $��cbEditGroupList��53   #   $
��Left=�6:   $��	10,16,0,0q�6U   $	��Top=�6h   $	��101��6�   �� 54      -   5        $	��Add��54   !   $
��Left=�6I   $��	10,43,0,0q�6d   $	��Top=�6w   $��49��6�   �� 55      -   5        $��Delete��55   !   $
��Left=�6O   $��	64,43,0,0q�6j   $	��Top=�6}   $��49��6�   57      �� 58      $��Admin��6   $��18(�6,   $
��Left=�6:   $��	341,0,0,0q�6U   $	��Top=�6h   �� 59      $��Logged In as ��6   $
��Left=�64   $��
323,32,0,0q�6O   $	��Top=�6c   �� 5:      -   5        $��	lUserName��5:      $��?��6-   $
��Left=�69   $��
395,32,0,0q�6T   $	��Top=�6h   � 5;      $
��Left=�6   $��41��66   $��472,295,-10,0q�6B   $	��Top=�6Y   $	��302��6q   .+�5<      �� $��Send System Messages via��6    $
��Left=�6C   $��0,-4,0,0q�6^   $	��Top=�6p   	� 5=      -   5         	�Name$ 
rEmailMode��5=   ?   $ grpSysMsgMode��6S   $��Email��6m   $
��Left=�6}   $��	153,2,0,0q�6�   $	��Top=�6�   	� 5>      -   5        $ 	rTextMode��5>   ?   $ grpSysMsgMode��6R   $
��Text��6l   $
��Left=�6{   $��	202,2,0,0q�6�   $	��Top=�6�   	� 5?      -   5        $ rOffMode��5?   ?   $ grpSysMsgMode��6Q   $	��Off��6k   $
��Left=�6y   $��	248,2,0,0q�6�   $	��Top=�6�   5@      �� 5A      $��System Settings��6   $
��Left=�66   $��472,261,0,0q�6Q   $	��Top=�6f   $��14(�6~   5E      5F      �� 5H      -   5        $
��Jobs��5H      $
��Left=�6]   $��41��6x   .��5I      � ���	#FFE5E5E5.+�5J      � $
��Left=�6   $	��358��66   $��
404,10,0,0q�6C   $	��Top=�6W   $	��251��6o   .+�5K      � $��32,51,35,187q�6    � ColumnDefinitions 5L      �� 5M   "   $
��157* �63   �� 5N   "   $	��27* �63   .+�5P      �� $��Weekly Jobs��6$   $
��Left=�6:   $	��Top=�6U   $��6,7,0,0q�6m   $��14(�6~   �� 5Q      -   5        $��cbWeekyJobs��5Q   '   $
��Left=�6:   $��	10,31,0,0q�6U   $	��Top=�6h   $	��164��6�   $��2��6�   �� 5R      -    5        $��
Run Single��5R   %   $
��Left=�6W   $��	10,58,0,0q�6r   $	��Top=�6�   $��69��6�   �� 5S      -!   5        $��Run All��5S   %   $
��Left=�6X   $��
105,58,0,0q�6s   $	��Top=�6�   $��69��6�   $��2��6�   �� 5T      -"   5        $	��Add��5T   %   $
��Left=�6S   $��	10,85,0,0q�6n   $	��Top=�6�   $��69��6�   �� 5U      -#   5        $
��Edit��5U   %   $
��Left=�6U   $��
105,85,0,0q�6p   $	��Top=�6�   $��69��6�   $��2��6�   5W      � 5X      $��32,193,35,45q�6   .+�5Y      �� $��Monthly Jobs��6$   $
��Left=�6;   $	��Top=�6V   $��6,7,0,0q�6n   $��14(�6   �� 5Z      -$   5        	 ��Name$	 cbMonthlyJobs��5Z   '   $��
10,31,10,0q�6>   $	��Top=�6R   �� 5[      -%   5        $��
Run Single��5[   %   $
��Left=�6X   $��	10,58,0,0q�6s   $	��Top=�6�   $��69��6�   �� 5\      -&   5        $��Run All��5\   %   $
��Left=�6Y   $��
106,58,0,0q�6t   $	��Top=�6�   $��69��6�   �� 5]      -'   5        $	��Add��5]   %   $
��Left=�6T   $��	10,86,0,0q�6o   $	��Top=�6�   $��69��6�   �� 5^      -(   5        $
��Edit��5^   %   $
��Left=�6V   $��
105,86,0,0q�6q   $	��Top=�6�   $��69��6�   5`      5a      � 5b      $
��Left=�6   $	��362��66   $��	95,22,0,0q�6C   $	��Top=�6V   $	��266��6n   .+�5c      � $��10,10,71,54q�6   .+�5d      �� $��Unscheduled Jobs��6$   $
��Left=�6?   $	��Top=�6Z   $��7,33,0,0q�6r   $��14(�6�   �� 5e      -)   5        $��cbUnscheduledJobs��5e   '   $
��Left=�6@   $��10,59,0,217q�6[   $	��164��6p   �� 5f      -*   5        $��
Run Single��5f   %   $
��Left=�6V   $��	10,86,0,0q�6q   $	��Top=�6�   $��69��6�   �� 5g      -+   5        $��Run All��5g   %   $
��Left=�6W   $��
105,86,0,0q�6r   $	��Top=�6�   $��69��6�   �� 5h      -,   5        $	��Add��5h   %   $
��Left=�6O   $��
10,113,0,0q�6j   $	��Top=�6~   $��69��6�   �� 5i      --   5        $
��Edit��5i   %   $
��Left=�6Q   $��105,113,0,0q�6l   $	��Top=�6�   $��69��6�   � 5j      $��
2,169,-1,9q�6#   .+�5k   "   �� $��
Daily Jobs��6(   $
��Left=�6=   $	��Top=�6X   $��6,7,0,0q�6p   $��14(�6�   �� 5l   "   -.   5        $	 cbDailyJobs��5l   +   $
��Left=�6@   $��
10,31,0,68q�6[   $	��164��6o   �� 5m   "   -/   5        $��
Run Single��5m   )   $
��Left=�6Z   $��	10,58,0,0q�6u   $	��Top=�6�   $��69��6�   �� 5n   "   -0   5        $��Run All��5n   )   $
��Left=�6[   $��
105,58,0,0q�6v   $	��Top=�6�   $��69��6�   �� 5o   "   -1   5        $	��Add��5o   )   $
��Left=�6V   $��	10,85,0,0q�6q   $	��Top=�6�   $��69��6�   �� 5p   "   -2   5        $
��Edit��5p   )   $
��Left=�6X   $��
105,85,0,0q�6s   $	��Top=�6�   $��69��6�   5r      5t      5u      5w      5x      �� 5y      -3   5        $��Texts��5y      .��5z      � ���	#FFE5E5E5
 +� FocusedElement
 6.   ��  �� ElementName$ tbTextLogNumber��.+�5{      	� -4   5        $��Outgoing��5{   "   $ rTextLog��6T   $
 TrueY�6i   $
��Left=�6z   $��	14,33,0,0q�6�   $	��Top=�6�   	� 5|      -5   5        $��Incoming��5|   "   $ rTextLog��6T   $
��Left=�6i   $��	89,33,0,0q�6�   $	��Top=�6�   �� 5}      -6   5        $��lMessageLogTitle��5}      $��Messages��64   $
��Left=�6G   $��
163,28,0,0q�6b   $	��Top=�6v   �� 5~      -7   5        $��lbOutgoingTexts��5~      $
��Left=�65   $	��154��6P   $��	10,51,0,0q�6]   $	��Top=�6p   $	��724��6�   �� 5      $��Failed Outgoing Messages��6   $
��Left=�6?   $��
11,203,0,0q�6Z   $	��Top=�6n   �� 5�      -8   5        $��lbFailedOutgoing��5�      $
��Left=�66   $	��154��6Q   $��
10,224,0,0q�6^   $	��Top=�6r   $	��724��6�   �� 5�      -9   5        $��tbTextLogNumber��5�   B   $
��Left=�6Y   $��22��6t   $��
504,28,0,0q�6�   $
 Wrap=�6�   $	��Top=�6�   $	��138��6�   �� 5�      $��Filter by Phone #��6   $
��Left=�68   $��
400,27,0,0q�6S   $	��Top=�6g   �� 5�      $��Texts��6   $��18(�6,   $
��Left=�6:   $��	340,0,0,0q�6U   $	��Top=�6h   5�      5�      �� 5�      -:   5        $��Reports��5�      .��5�      � -;   5        $��gridReports��5�      ���	#FFE5E5E5 5�      �� 5�      $��4* �6+   �� 5�      $
��187* �6+   .+�5�      &   System.Windows.Controls.Calendar  -<   5        $��	calTotals��5�      $
��Left=�6V   $��
115,86,0,0q�6q   $	��Top=�6�   $��1��6�   �� 5�      $��Select Day to View Totals��6   $
��Left=�6@   $��
118,66,0,0q�6[   $	��Top=�6o   $��1��6�   �� 5�      -=   5        $��cbReportType��5�       $
��Left=�64   $��
463,94,0,0q�6O   $	��Top=�6c   $	��100��6{   $��1��6�   .@�5�      �� $��Weekly��6'   $
��Left=�68   $��88��6S   �� 5�      $��Monthly��6'   $
��Left=�69   $��88��6T   �� 5�      $��Yearly��6'   $
��Left=�68   $��88��6S   5�      �� 5�      $��Select Report��6   $
��Left=�64   $��
465,68,0,0q�6O   $	��Top=�6c   $��1��6{   	� 5�      ->   5        $��rbEmail��5�   "   $��Email��61   $ sendReportTo��6A   $
 TrueY�6Z   $��1��6k   $
��Left=�6{   $��465,126,0,0q�6�   $	��Top=�6�   	� 5�      -?   5        $��	rbPrinter��5�   "   $��Printer��63   $ sendReportTo��6E   $��1��6^   $
��Left=�6n   $��465,154,0,0q�6�   $	��Top=�6�   �� 5�      -@   5        $��Generate��5�      $��1��6P   $
��Left=�6`   $��463,187,0,0q�6{   $	��Top=�6�   $��75��6�   �� 5�      $��Reports��6   $��18(�6.   $
��Left=�6<   $��	324,0,0,0q�6W   $	��Top=�6j   $��1��6�   �� 5�      -A   5        $��lbOutgoingTotals��5�      $��1��64   $��Center=�6D   $��Center=�6a   $��118,274,567,0q�6�   $	��Top=�6�   $��63��6�   $��26��6�   �� 5�      -B   5        $��lbIncomingTotals��5�      $��1��64   $��Center=�6D   $��Center=�6a   $��223,274,454,0q�6�   $	��Top=�6�   $��71��6�   $��26��6�   5�      5�      �� 5�      -C   5        $��Modem��5�      .��5�      � ���	#FFE5E5E5.+�5�      �� -D   5         ��Name$ 
lbModemLog��5�      $
��Left=�62   $	��304��6M   $��	10,70,0,0q�6Z   $	��Top=�6m   $	��380��6�   �� 5�      $��	Modem Log��6   $
��Left=�60   $��	10,36,0,0q�6K   $	��Top=�6^   �� 5�      -E   5        $��cbModemSelect��5�      $
��Left=�64   $��	99,36,0,0q�6O   $	��Top=�6b   $	��102��6z   $��20��6�   5�      5�      �� 5�      -F   5        $
��Info��5�      .��5�      � ���	#FFE5E5E5 5�      �� 5�      $	��83* �6+   �� 5�      $
��299* �6+   .+�5�      �� $��	Socket In��6   $
��Left=�60   $��18,8,0,0q�6K   $	��Top=�6]   �� 5�      $��
Socket Out��6   $
��Left=�61   $��
18,193,0,0q�6L   $	��Top=�6`   �� 5�      -G   5        $��lbSockIn��5�      $
��Left=�6.   $	��165��6I   $��	15,28,0,0q�6V   $	��Top=�6i   $	��310��6�   $��2��6�   �� 5�      -H   5        $��	lbSockOut��5�      $
��Left=�6/   $	��165��6J   $��
15,213,0,0q�6W   $	��Top=�6k   $	��310��6�   $��2��6�   �� 5�      -I   5        $ lbErrLog��5�      $
��Left=�60   $	��348��6K   $��
260,26,0,0q�6X   $	��Top=�6l   $	��310��6�   $��1��6�   �� 5�      $��Console Exceptions��6   $
��Left=�69   $��263.25,6,0,0q�6T   $	��Top=�6j   $��1��6�   �� 5�      $
��Info��6   $��18(�6+   $
��Left=�69   $��174.25,0,0,0q�6T   $	��Top=�6j   $��1��6�   5�      5�      5�      5�      5�      