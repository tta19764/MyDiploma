   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5      ,    &HumanResourcesApp.Views.AttendancePage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5      =controls(clr-namespace:HumanResourcesApp.Controls     5        N� Title$  Attendance Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverterC   =HumanResourcesApp.Classes.InverseBooleanToVisibilityConverter  InverseBoolToVisConverter  DataGridColumnHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value  ActionButton �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  DeleteButton �� BasedOn �� Triggers  
SaveButton  CancelButton��%�  &	        &	      &	 &     &	 =    &	 �    7  &	     7  &	 B    7    5   
   6/     5   
   66   �� 5   
   "  62   .�5      ��  � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$
 10,8q�6(   �� 5       � ��$ 0,0,1,0q�60   �� 5       � �� �#4393E46,   5      �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 0,0,8,0q�6'   �� 5       � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5        � ��$ 0q�60   �� 5!       �@�Cursor$
 Hand��6'   �� 5"       � �� 5#      �� 5$      " ��6&   .R�5%      �� #��z��6!   #��z��5&      #��z��5'      $ 4��5(      ._�5)      �� $��Center=�6/   $��Center=�6L   5*       5+      P� 5,      	 �@J�IsMouseOver$

 True��6=   . �5-   "   ��  � �� �#1D75C06?   5.      P� 5/      	 � ��$

 True��6;   . �50   "   ��  � �� �#166ABD6?   51      53      55      56      �� 58   
   " ��6%   8   .�59      ��  � �� �#FF52526+    5:      P� 5;      	 �@J�IsMouseOver$

 True��61   . �5<      ��  � �� �#E03E3E63   5=      P� 5>      	 � ��$

 True��6/   . �5?      ��  � �� �#C1333363   5@      5B      �� 5D   
   " ��6#   8   .�5E      ��  � �� �#27AE606+    5F      P� 5G      	 �@J�IsMouseOver$

 True��61   . �5H      ��  � �� �#22995463   5I      P� 5J      	 � ��$

 True��6/   . �5K      ��  � �� �#1E874B63   5L      5N      �� 5P   
   " ��6%   8   .�5Q      ��  � �� �#7F8C8D6+    5R      P� 5S      	 �@J�IsMouseOver$

 True��61   . �5T      ��  � �� �#6C7A7B63   5U      P� 5V      	 � ��$

 True��6/   . �5W      ��  � �� �#5E696A63   5X      5Z      5[      .��5]      �  � RowDefinitions 5^   
   �� 5_      $
��Auto �6   �� 5`      $��* �6   �� 5a      $
��Auto �6   .+�5e   
   �� $��0��6   ���#2E75B6 �� Padding$ 15q�63   ._�5f      �  � ColumnDefinitions 5g      �� 5h      $��* �6'   �� 5i      $
��Auto �6'   �� 5j      $
��Auto �6'   .+�5m      �� $��Attendances��6   $��22(�5n      $
��Bold"�5o      
�������$��2��5p   $   4   .HumanResourcesApp.Controls.ActionButtonControl  5r      $��1��5s      $��Right=�5t         ButtonContent$ Add Attendance��5u         ButtonPadding$
 12,8q�5w         ButtonCommand 5v      �� *AddAttendanceCommand+y�5x      �� *CanManageAttendances+ �� 	Converter# [    5z      $��10,0,0,0q�5{      $��2��5|      $��Right=�5}      $
 12,8q�5�      y�5~      �� *
IsEmployee+# [   5      �� *CheckAction+ 5�      �� *CheckCommand+5�      5�      5�      �� 5�   
   $��1��6   
�������$ 8��61   ._�5�      &   System.Windows.Controls.DataGrid  -   5          Name$ AttendanceDataGrid��5�         AutoGenerateColumns$ False��5�         
IsReadOnly$
 True��5�         CanUserAddRows$ False��5�         RowHeaderWidth$ 0Q�5�      $��1q�5�      ���#DDDDDD$��0,0,0,20q�5�      
�������   RowBackground
 �����   AlternatingRowBackground �#F9FAFC   GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�         HorizontalGridLinesBrush �#E3E3E3   MinRowHeight$ 40Q�5�         ColumnHeaderStyle# [ 5�      ��5�      �� *Attendances+  �� SelectedItem  5�      �� *SelectedAttendance+!   Columns! 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn"   Header$" Employee��6)   #   Width5  /System.Windows.Controls.DataGridLengthConverter$# 1* 6;   1	  +System.Windows.Controls.DataGridBoundColumn$ 	  Binding$ 5�   )   �� *EmployeeFullName+% 	  ElementStyle% 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $" Check In��6)   $# 1* 6;   $ 5�   (   �� *CheckInTime+& �� StringFormat$& {0:yyyy-MM-dd HH:mm}��% 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $" 	Check Out��6)   $# 1* 6<   $ 5�   (   �� *CheckOutTime+$& {0:yyyy-MM-dd HH:mm}��% 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $" Status��6)   $# 1* 69   $ 5�      �� *Status+% 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $" Notes��6)   $# 1* 68   $ 5�      �� *Notes+% 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�      4
  .System.Windows.Controls.DataGridTemplateColumn
  5�      $" Actions��6-   $
# auto 6>   ' 
  CellTemplate' 5�      �� 5�      .R�5�   "   �� ( �� Orientation$( 
Horizontal=�6-   $	��4,4q�6F   y�5�   -   �� *" DataContext.CanManageAttendances+) �� RelativeSource) �� * �� AncestorType"*  # [  .+�5�   &   �� $��70��6-   $
��Edit��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *#!DataContext.EditAttendanceCommand+) �� "*  ��5�   )   �� �� 5�   &   $��70��6-   $��Delete��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *%#DataContext.DeleteAttendanceCommand+) �� "*  ��5�   )   �� 5�   #   5�      5�      5�      5�      �� 5�   
   
�������$ 20q�6%   $ 8��62   $��15 20 15 15q�6C   $��2��6X   y�5�      �� *IsAddingNewOrEditing+# [  ._�5�      �� $( Vertical=�6   $	��450��60   .+�5�      �� $��18(�6;   $
��Bold"�6I   $��0 0 0 10q�6[   ��6   �� *EditAddHeader+6[   �� 5�      $( 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��Department:��6    $	��120��63   $��Center=�6?   �� 5�      $	��300��6   + �� DisplayMemberPath$+ DepartmentName��5�      ��5�      �� *Departments+  5�      �� *SelectedDepartment+5�      5�      �� 5�      $( 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��	Employee:��6    $	��120��61   $��Center=�6=   �� 5�      $	��300��6   $+ FullName��5�      ��5�      �� *	Employees+  5�      �� *SelectedEmployee+5�      5      �� 5     $( 
Horizontal=�6   $	��0 5q�66   .+�5     �� $��Check In Time:��6    $	��120��66   $��Center=�6B   (  "System.Windows.Controls.DatePicker  5     $	��120��5     ,   SelectedDate, 5  !   �� *NewAttendance.CheckInTime+- �� UpdateSourceTrigger$- PropertyChanged=�5     �� 5     -   5        . ��Name$. CheckInTimeTextBox��5     $	��100��6:   $	��5 0q�6F   / � ToolTip$ / Format: HH:mm (e.g. 08:30)��5     ��5     �� *CheckInTimeText+$- PropertyChanged=�5     5	     �� 5     $( 
Horizontal=�6   $	��0 5q�66   .+�5     �� $��Check Out Time:��6    $	��120��67   $��Center=�6C     5     $	��120��5     , 5  !   �� *NewAttendance.CheckOutTime+$- PropertyChanged=�5     �� 5     -   5        $. CheckOutTimeTextBox��5     $	��100��6;   $	��5 0q�6G   $ / Format: HH:mm (e.g. 17:30)��5     ��5     �� *CheckOutTimeText+$- PropertyChanged=�5     5     �� 5     $( 
Horizontal=�6   $	��0 5q�66   .+�5     �� $��Status:��6    $	��120��6/   $��Center=�6;   � 5      5     �� 5     $	��300 �6/   �� 5     $
��Auto �6/   .+�5     �� $	��300��6#   $��0��6/   ��5     �� *
Statuses+  5     �� *NewAttendance.Status+5     5     �� 5!     $( 
Horizontal=�6   $	��0 5q�66   .+�5"     �� $��Notes:��6    $	��120��6.   $	��Top=�6:   �� 5#     $	��300��6C   $��60��6O   0 �� TextWrapping$
0 Wrap=�6[   1 �� AcceptsReturn$
1 True��6o   ��6   �� *NewAttendance.Notes+6o   5$     �� 5&     $( 
Horizontal=�5'     $��
0,10,20, 0q�5(     $��Right=�5)     .+�5*     �� $
��Save��6   #��[ 5,     $	��100��5-     $��0,0,10,0q�5.     $��20��5/     ��5+     �� *SaveNewAttendanceCommand+5/     �� 51     $��Cancel��6   #��[ 53     $	��100��54     $��20��55     ��52     �� *CancelCommand+55     56     57     58     59     5:     