   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5      ,    &HumanResourcesApp.Views.PayPeriodsPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5      =controls(clr-namespace:HumanResourcesApp.Controls     5        N� Title$  Pay Period Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverterC   =HumanResourcesApp.Classes.InverseBooleanToVisibilityConverter  InverseBoolToVisConverter;   5HumanResourcesApp.Classes.DateOnlyToDateTimeConverter  DateOnlyConverter  DataGridColumnHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value  ActionButton �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  DeleteButton �� BasedOn �� Triggers  
SaveButton  CancelButton  StatusStyle �� Binding �� Value��%�	  &	        &	      &	 &     &	 9     &	 P    &	     7  &	 1    7  &	 U    7  &	 y      5   
   6/     5   
   66     5   
   6.   �� 5   
   "  62   .�5      ��  � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$
 10,8q�6(   �� 5       � ��$ 0,0,1,0q�60   �� 5       � �� �#4393E46,   5      �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 0,0,8,0q�6'   �� 5       � �� �#2E86DE6+   �� 5        � ��
 �����6+   �� 5!       � ��$ 0q�60   �� 5"       �@�Cursor$
 Hand��6'   �� 5#       � �� 5$      �� 5%      " ��6&   .R�5&      �� #��z��6!   #��z��5'      #��z��5(      $ 4��5)      ._�5*      �� $��Center=�6/   $��Center=�6L   5+       5,      P� 5-      	 �@J�IsMouseOver$

 True��6=   . �5.   "   ��  � �� �#1D75C06?   5/      P� 50      	 � ��$

 True��6;   . �51   "   ��  � �� �#166ABD6?   52      54      56      57      �� 59   
   " ��6%   8   .�5:      ��  � �� �#FF52526+    5;      P� 5<      	 �@J�IsMouseOver$

 True��61   . �5=      ��  � �� �#E03E3E63   5>      P� 5?      	 � ��$

 True��6/   . �5@      ��  � �� �#C1333363   5A      5C      �� 5E   
   " ��6#   8   .�5F      ��  � �� �#27AE606+    5G      P� 5H      	 �@J�IsMouseOver$

 True��61   . �5I      ��  � �� �#22995463   5J      P� 5K      	 � ��$

 True��6/   . �5L      ��  � �� �#1E874B63   5M      5O      �� 5Q   
   " ��6%   8   .�5R      ��  � �� �#7F8C8D6+    5S      P� 5T      	 �@J�IsMouseOver$

 True��61   . �5U      ��  � �� �#6C7A7B63   5V      P� 5W      	 � ��$

 True��6/   . �5X      ��  � �� �#5E696A63   5Y      5[      �� 5]   
   " ��6$    5^      �� 5_       6   �� *Status+	 Draft69   .a�5`      ��  � �� �#6C7A7B63   �� 5a       � ��$ Italic$�62   5b      �� 5c       6   �� *Status+
 Active69   .a�5d      ��  � �� �#2E86DE63   �� 5e       � ��$ SemiBold"�63   5f      �� 5g       6   �� *Status+ 	Completed69   .a�5h      ��  � �� �#27AE6063   �� 5i       � ��$ SemiBold"�63   5j      5l      5m      .��5o      �  � RowDefinitions 5p   
   �� 5q      $
��Auto �6   �� 5r      $��* �6   �� 5s      $
��Auto �6   .+�5w   
   �� $��0��6   ���#2E75B6 �� Padding$ 15q�63   ._�5x      �  � ColumnDefinitions 5y      �� 5z      $��* �6'   �� 5{      $
��Auto �6'   .+�5~      �� $��Pay Periods��6   $��22(�5      $
��Bold"�5�      
�������$��2��5�   (   4   .HumanResourcesApp.Controls.ActionButtonControl  5�      $��1��5�      $��Right=�5�         ButtonContent$ Add Pay Period��5�         ButtonPadding$
 12,8q�5�         ButtonCommand 5�      �� *AddPayPeriodCommand+y�5�      �� *CanProcessPayroll+ �� 	Converter# [  5�      5�      �� 5�   
   $��1��6   
�������$ 8��61   ._�5�      &   System.Windows.Controls.DataGrid  -   5          Name$ PayPeriodDataGrid��5�         AutoGenerateColumns$ False��5�         
IsReadOnly$
 True��5�         CanUserAddRows$ False��5�         RowHeaderWidth$ 0Q�5�      $��1q�5�      ���#DDDDDD$��0,0,0,20q�5�      
�������   RowBackground
 �����   AlternatingRowBackground �#F9FAFC   GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�         HorizontalGridLinesBrush �#E3E3E3    ColumnHeaderStyle#  [ 5�      !   	RowHeight$! 40Q�5�      ��5�      �� *
PayPeriods+" �� SelectedItem" 5�      �� *SelectedPayPeriod+#   Columns# 5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      ,  &System.Windows.Controls.DataGridColumn$   Header$$ 
Start Date��6-   %   Width5	  /System.Windows.Controls.DataGridLengthConverter$	% .5*	 6A   &   CellTemplate& 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6l   ��60   �� *	StartDate+' �� StringFormat$' {0:dd/MM/yyyy}��6l   5�   #   5�      5�        5�      $$ End Date��6-   $	% .5*	 6?   & 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6j   ��60   �� *	EndDate+$' {0:dd/MM/yyyy}��6j   5�   #   5�      5�        5�      $$ Payment Date��6-   $	% .5*	 6C   & 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6n   ��60   �� *PaymentDate+$' {0:dd/MM/yyyy}��6n   5�   #   5�      5�        5�      $$ Status��6-   $	% .5*	 6=   & 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� #��[ 6H   $��Center=�6m   ��60   �� *Status+6m   5�   #   5�      5�        5�      $$ Associated Payrolls��6-   $	% .5*	 6J   & 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6N   ��60   �� *PayrollCount+6N   5�   #   5�      5�        5�      $$ Actions��6-   $
% auto	 6>   & 5�      �� 5�      .R�5�   "   �� ( �� Orientation$( 
Horizontal=�6-   $	��4,4q�6F   y�5�   -   �� *DataContext.CanProcessPayroll+) �� RelativeSource) �� * �� AncestorType"*  # [  .+�5�   &   �� $��70��6-   $
��Edit��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *" DataContext.EditPayPeriodCommand+) �� "*  ��5�   )   �� y�5�   .   �� *
IsEditable+# [  {�5�   )   �� *
IsEditable+�� 5�   &   $��70��6-   $��Delete��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *$"DataContext.DeletePayPeriodCommand+) �� "*  ��5�   )   �� y�5�   -   �� *IsDeletable+# [  {�5�   )   �� *IsDeletable+5�   #   5�      5�      5�      5�      �� 5�   
   
�������$ 20q�6$   $ 8��61   $��15 20 15 15q�6B   $��2��6W   y�5�      �� *IsAddingOrEditing+# [  ._�5�      �� $( Vertical=�6   $	��450��60   .+�5�      �� $��18(�67   $
��Bold"�6E   $��0 0 0 10q�6W   ��6   �� *	FormTitle+6W   �� 5�      $( 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��Start Date:��6    $	��120��63   $��Center=�6?   (
  "System.Windows.Controls.DatePicker
  5�      $	��300��5�      + 
  SelectedDate+ 5�   !   �� *NewPayPeriod.StartDate+, �� UpdateSourceTrigger$, PropertyChanged=�# [ 5�      5�      �� 5�      $( 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��	End Date:��6    $	��120��61   $��Center=�6=   
  5�      $	��300��5�      + 5�   !   �� *NewPayPeriod.EndDate+$, PropertyChanged=�# [ 5�      5      �� 5     $( 
Horizontal=�6   $	��0 5q�66   .+�5     �� $��Payment Date:��6    $	��120��65   $��Center=�6A   
  5     $	��300��5     + 5  !   �� *NewPayPeriod.PaymentDate+$, PropertyChanged=�# [ 5     5     �� 5     $( 
Horizontal=�6   $	��0 5q�66   .+�5	     �� $��Status:��6    $	��120��6/   $��Center=�6;   �� 5
     $	��300��6D   - �� Text- 6   �� *NewPayPeriod.Status+.@�5     �� $��Draft��6'   �� 5     $��Active��6'   �� 5     $��	Completed��6'   5     5     �� 5     $( 
Horizontal=�5     $��	0,10,20,0q�5     $��Right=�5     .+�5     �� $
��Save��6   #��[ 5     $	��100��5     $��0,0,10,0q�5     $��20��5     ��5     �� *SaveCommand+5     �� 5     $��Cancel��6   #��[ 5     $	��100��5     $��20��5      ��5     �� *CancelCommand+5      5!     5"     5#     5$     5%     