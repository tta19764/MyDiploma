   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5   	   1    +HumanResourcesApp.Views.TimeOffRequestsPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5      =controls(clr-namespace:HumanResourcesApp.Controls     5   	     N� Title$  Attendance Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverter<   6HumanResourcesApp.Classes.PendingAndCanManageConverter   PendingAndCanManageConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  ReviewButton �� BasedOn �� Triggers  DeleteButton��%�  &	        &	      &	 &     &	 O    7  &	 s    7    5   
   6/     5   
   6/   �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��$ 30��6'   �� 5       � ��$ 75��6)   �� 5       � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$ 0q�60   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5       #��z��5!      $ 4��5"      ._�5#      �� $��Center=�6/   $��Center=�6L   5$       5%      P� 5&      	 �@J�IsMouseOver$

 True��6=   . �5'   "   ��  � �� �#1D75C06?   5(      P� 5)      	 � ��$

 True��6;   . �5*   "   ��  � �� �#166ABD6?   5+      5-      5/      50      �� 52   
   " ��6%   8   .�53      ��  � �� �#4CAF506+    54      P� 55      	 �@J�IsMouseOver$

 True��61   . �56      ��  � �� �#43A04763   57      P� 58      	 � ��$

 True��6/   . �59      ��  � �� �#388E3C63   5:      5<      �� 5>   
   " ��6%   8   .�5?      ��  � �� �#FF52526+    5@      P� 5A      	 �@J�IsMouseOver$

 True��61   . �5B      ��  � �� �#E03E3E63   5C      P� 5D      	 � ��$

 True��6/   . �5E      ��  � �� �#C1333363   5F      5H      5J      .��5K   
   �  � RowDefinitions 5L   
   �� 5M      $
��Auto �6   �� 5N      $��* �6   �� 5O      $
��Auto �6    � ColumnDefinitions 5Q   
   �� 5R      $��* �6   �� 5S      $
��Auto �6   .+�5U   
   �� ���#2E75B6 �� Padding$ 15q�6&   $��0��63   ._�5V      ]� .+�5W      �� $��0��6   $��Time Off Requests��6,   $��22(�5X      $
��Bold"�5Y      
�������$��Center=�5[      �� 5\      $��1��6    �� Orientation$ 
Horizontal=�6-   $��Right=�5]      $��Right=�65   .+�5^      4   .HumanResourcesApp.Controls.ActionButtonControl  $��Right=�5_         ButtonContent$ New Request��5`         ButtonPadding$
 12,8q�5b      $��0 0 15 0q�5c         ButtonCommand 5a      �� *CreateCommand+5c        5e      $��Right=�5f      $ Refresh��5g      $
 12,8q�5i      $��0 0 10 0q�5j       5h      �� *RefreshCommand+5j      5k      5l      5m      �� 5n   
   $��1��6   .��5o      �� ���#F7F7F7.+�5q      �� ��6   �� *TimeOffRequestsView+��5r      �� 5s      .R�5t      �� ���#DDD$��1q�5u   !   $ 8��5v   !   $��
15,15,15,0q�5w   !   $ 16q�5x   !   
�������._�5z   "   �  5{   &   �� 5|   *   $
��Auto �68   �� 5}   *   $
��Auto �68   �� 5~   *   $
��Auto �68    5�   &   �� 5�   *   $��* �6;   �� 5�   *   $
��Auto �6;   .+�5�   &   �� $��0��61   $��0��6>   $ 
Horizontal=�6N   .+�5�   *   �� $��18(�6R   $
��Bold"�6`   ���#333��64   �� *EmployeeName+6r   �� 5�   *   $�� – ��64   $��18(�6?   $	��4,0q�6M   �� 5�   *   $��18(�6U   $
��Bold"�6c   ���#2E86DE��64   �� *TimeOffTypeName+6u   5�   '   �� 5�   &   $��1��61   $��0��6>   $��0,8,0,0q�6N   .+�5�   *   �� �� 5�   .   $�Period: ��62    �� 5�   .   $��SemiBold"�6M   �62   �� *	StartDate+6M    �� 5�   .   $� – ��62    �� 5�   .   $��SemiBold"�6K   �62   �� *	EndDate+6K    �� 5�   .   $� (��62    �� 5�   .   �62   �� *	TotalDays+ �� 5�   .   $� days)��62   5�   +   �� 5�   *   $��Italic$�6L   ���#666$
��Wrap=�6q   $��0,4,0,0q�6�   ��64   �� *Reason+6�   5�   '   �� 5�   &   $��2��61   $��0��6>   $ 
Horizontal=�6N   $��0,10,0,0q�6g   .+�5�   *   �� �� 5�   .   $�Status: ��62   $��SemiBold"�6B    �� 5�   .   ���#444�62   �� *Status+6J   5�   +   �� 5�   *   $	�� | ��64   $	��6,0q�6?   �� 5�   *   �� 5�   .   $�Reviewed by: ��62    �� 5�   .   $��SemiBold"�6R   �62   �� *ApprovedByName+6R   5�   +   �� 5�   *   $	�� | ��64   $	��6,0q�6?   �� 5�   *   �� 5�   .   $�Review Date: ��62    �� 5�   .   $��SemiBold"�6P   �62   �� *ApprovalDate+6P   5�   +   5�   '   �� 5�   &   $��3��61   $��1��6B   $ Vertical=�6R   $��Right=�6i   $��Center=�6�   $��10,0,0,0q�6�   .+�5�   *   �� $
��Edit��61   ��5�   -   �� *DataContext.EditCommand+ �� RelativeSource ��  �� AncestorType Page��5�   -   �� ��5�   .   �� 5�   2   " ��68   # [ 6L   .�5�   6   ��  � y�$ 	Collapsed=�6S    5�   6   �� 5�   :    �� Binding 6F   �� *Status+ �� Value Pending6a   .a�5�   >   ��  � y�$ Visible=�6[   5�   ;   5�   3   5�   +   �� 5�   *   $��Review��61   ��5�   -   �� *DataContext.ReviewCommand+ ��  Page��5�   -   �� ��5�   .   �� 5�   2   " ��68   # [ 6L   .�5�   6   ��  � y�$ 	Collapsed=�6S    5�   6   �� 5�   :    Visible6F    5�   >   `� 5�   B    `� 	Converter# [ 6O   .1�5�   F   ��  �� Path$ Status�6N   �� 5�   F   $# DataContext.CanManageTimeOffs�6N    6s   ��  ItemsControl5�   C   .a�5�   >   ��  � y�$ Visible=�6[   5�   ;   5�   3   5�   +   �� 5�   *   $��Delete��61   #��[ 5�      ��5�      �� *)'DataContext.DeleteTimeOffRequestCommand+ ��  Page��5�      �� 5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      