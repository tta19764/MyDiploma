   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      K*clr-namespace:HumanResourcesApp.ViewModelsHumanResourcesApp.ViewModels  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5	      '    !HumanResourcesApp.Views.UsersPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5      9vm*clr-namespace:HumanResourcesApp.ViewModels     5      =controls(clr-namespace:HumanResourcesApp.Controls     5	        N� Title$  User Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverterC   =HumanResourcesApp.Classes.InverseBooleanToVisibilityConverter  InverseBoolToVisConverter;   5HumanResourcesApp.Classes.BoolToActiveStatusConverter  BoolToActiveConverter9   3HumanResourcesApp.Classes.DateTimeToStringConverter  DateTimeConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  DeleteButton �� BasedOn �� Triggers  	AddButton  DataGridColumnHeaderStyle=  7System.Windows.Controls.Primitives.DataGridColumnHeader  EditTextBox��%�  &	        &	      &	 &     &	 9     &	 L     &	 '    7  &	 K    7  &	 �    &	 �      5   
   6/     5   
   66     5   
   6.     5   
   6,   �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 2q�6'   �� 5       � ��$ 22��6'   �� 5       � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5       " ��6&   .R�5!      �� #��z��6!   #��z��5"      #��z��5#      $ 4��5$      ._�5%      �� $��Center=�6/   $��Center=�6L   5&       5'      P� 5(      	 �@J�IsMouseOver$

 True��6=   . �5)   "   ��  � �� �#1D75C06?   5*      P� 5+      	 � ��$

 True��6;   . �5,   "   ��  � �� �#166ABD6?   5-      5/      51      52      �� 54   
   " ��6%   8   .�55      ��  � �� �#FF52526+    56      P� 57      	 �@J�IsMouseOver$

 True��61   . �58      ��  � �� �#E03E3E63   59      P� 5:      	 � ��$

 True��6/   . �5;      ��  � �� �#C1333363   5<      5>      �� 5@   
   " ��6"   8   .�5A      ��  � ��$ 40��6'   �� 5B       � ��$ SemiBold"�6+   5C      �� 5F   
   "  62   .�5G      ��  � �� �#2E86DE6+   �� 5H       � ��
 �����6+   �� 5I       � ��$ SemiBold"�6+   �� 5J       � ��$
 10,8q�6(   �� 5K       � ��$ 0,0,1,0q�60   �� 5L       � �� �#4393E46,   5M      �� 5P   
   " ��6$   .�5Q      ��  � ��$	 5,3q�6(   �� 5R       � �� �#CCCCCC6,   �� 5S       � ��$ 1q�60   5T      5U      .��5W      �� $��0��6   
������ ._�5X   
   �  � RowDefinitions 5Y      �� 5Z      $
��Auto �6    �� 5[      $��* �6    .+�5_      �� $��0��6   ���#2E75B6 �� Padding$ 15q�67   ._�5`      �  � ColumnDefinitions 5a      �� 5b      $��* �6+   �� 5c      $
��Auto �6+   .+�5f      �� $��Users��6    $��22(�5g       $
��Bold"�5h       
�������5i       4   .HumanResourcesApp.Controls.ActionButtonControl  5k      $��1��5l      $��Right=�5m         ButtonContent$ Add New User��5n         ButtonPadding$
 12,8q�5p         ButtonCommand 5o      �� *AddUserCommand+y�5q      �� *CanManageUsers+ �� 	Converter# [  5r      5s      &   System.Windows.Controls.DataGrid  5v      $��1��6      AutoGenerateColumns$ False��5y         
IsReadOnly$
 True��5z         SelectionMode$ Single=�5{         CanUserAddRows$ False��5|         CanUserDeleteRows$ False��5}      $��1q�5~      ���#DDDDDD$��0,0,0,20q�5�      
�������   RowBackground
 �����   AlternatingRowBackground �#F9FAFC   GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�         HorizontalGridLinesBrush �#E3E3E3   ColumnHeaderStyle# [ 5�      ��5w      �� *Users+ �� SelectedItem 5x      �� *SelectedUser+    Columns  5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,	  &System.Windows.Controls.DataGridColumn! 	  Header$! ID��6)   " 	  Width5
  /System.Windows.Controls.DataGridLengthConverter$" 50
 6P   1  +System.Windows.Controls.DataGridBoundColumn#   Binding# 65   �� *UserId+$   ElementStyle$ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   �� 5�   "    � ��$ SemiBold"�6?   5�      5�        5�      $! Username��6)   $" *
 6X   # 6;   �� *
Username+$ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $! 
First Name��6)   $" *
 6=   # 5�      �� 5�      % �� Path$% Employee.FirstName�6&   & �� FallbackValue$	& N/A��6@   ' �� TargetNullValue$	' N/A��5�   W   5�      $ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $! 	Last Name��6)   $" *
 6<   # 5�      �� 5�      $% Employee.LastName�6&   $	& N/A��6?   $	' N/A��5�   W   5�      $ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $! Email��6)   $" *
 68   # 5�      �� 5�      $% Employee.Email�6&   $	& N/A��6<   $	' N/A��5�   W   5�      $ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $
! Role��6)   $" *
 6Y   # 67   �� *Role.RoleName+$ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   �� 5�   "    � ��$
 Wrap=�6A   5�      5�        5�      $! 
Last Login��6)   $" *
 6�   # 6=   �� *	LastLogin+# [ $ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   5�      5�        5�      $! Status��6)   $
" auto
 6�   # 69   �� *
IsActive+# [ $ 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<    5�   "   �� 5�   &   ( �� Binding( 62   �� *
IsActive+) �� Value) True6O   .a�5�   *   ��  � �� �#2ECC716G   �� 5�   *    � ��$ SemiBold"�6G   5�   '   �� 5�   &   ( 62   �� *
IsActive+	) False6O   .a�5�   *   ��  � �� �#E74C3C6G   �� 5�   *    � ��$ SemiBold"�6G   5�   '   5�      5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      $! Actions��6-   $
" auto
 6>   *   CellTemplate* 5�      �� 5�      .R�5�   "   � $��5q�6'   .+�5�   &   �� + �� Orientation$+ Vertical=�61   $	��Top=�6H   y�6`   �� *DataContext.CanManageUsers+, �� RelativeSource, �� - �� AncestorType- DataGrid# [  .+�5�   *   �� $��70��61   $
��Edit��5�   1   #��[ 5�   1   ��5�   1   �� *DataContext.EditUserCommand+, �� - DataGrid��5�   1   �� �� 5�   *   $��70��61   $��Delete��5�   1   #��[ 5�   1   ��5�   1   �� *DataContext.DeleteUserCommand+, �� - DataGrid��5�   1   �� 5�   '   5�   #   5�      5�      5�      5�      5�      5�      