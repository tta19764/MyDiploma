   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      *    $HumanResourcesApp.Views.PositionPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5        N� Title$  Position Management��5
       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverterC   =HumanResourcesApp.Classes.InverseBooleanToVisibilityConverter  InverseBoolToVisConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  DeleteButton �� BasedOn �� Triggers  
SaveButton  CancelButton  	AddButton  DataGridColumnHeaderStyle=  7System.Windows.Controls.Primitives.DataGridColumnHeader  EditTextBox��%�  &	        &	      &	 &     &	 �    7  &	     7  &	 +    7  &	 O    7  &	 �    &	 �      5   
   6/     5   
   66   �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 0,0,8,0q�6'   �� 5       � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $ 4��5   !   ._�5       �� $��Center=�6/   $��Center=�6L   5!       5"      P� 5#      	 �@J�IsMouseOver$

 True��6=   . �5$   "   ��  � �� �#1D75C06?   5%      P� 5&      	 � ��$

 True��6;   . �5'   "   ��  � �� �#166ABD6?   5(      5*      5,      5-      �� 5/   
   " ��6%   8   .�50      ��  � �� �#FF52526+    51      P� 52      	 �@J�IsMouseOver$

 True��61   . �53      ��  � �� �#E03E3E63   54      P� 55      	 � ��$

 True��6/   . �56      ��  � �� �#C1333363   57      59      �� 5;   
   " ��6#   8   .�5<      ��  � �� �#27AE606+    5=      P� 5>      	 �@J�IsMouseOver$

 True��61   . �5?      ��  � �� �#22995463   5@      P� 5A      	 � ��$

 True��6/   . �5B      ��  � �� �#1E874B63   5C      5E      �� 5G   
   " ��6%   8   .�5H      ��  � �� �#7F8C8D6+    5I      P� 5J      	 �@J�IsMouseOver$

 True��61   . �5K      ��  � �� �#6C7A7B63   5L      P� 5M      	 � ��$

 True��6/   . �5N      ��  � �� �#5E696A63   5O      5Q      �� 5S   
   " ��6"   8   .�5T      ��  � ��$ 40��6'   �� 5U       � ��$ SemiBold"�6+   5V      �� 5Y   
   "  62   .�5Z      ��  � �� �#2E86DE6+   �� 5[       � ��
 �����6+   �� 5\       � ��$ SemiBold"�6+   �� 5]       � ��$
 10,8q�6(   �� 5^       � ��$ 0,0,1,0q�60   �� 5_       � �� �#4393E46,   5`      �� 5c   
   " ��6$   .�5d      ��  � ��$	 5,3q�6(   �� 5e       � �� �#CCCCCC6,   �� 5f       � ��$ 1q�60   5g      5h      .��5j      �� $��0��6   
������ ._�5k   
   �  � RowDefinitions 5l      �� 5m      $
��Auto �6    �� 5n      $��* �6    �� 5o      $
��Auto �6    .+�5s      �� $��0��6   ���#2E75B6 �� Padding$ 15q�67   ._�5t      �� $��	Positions��6   $��22(�5u      $
��Bold"�5v      
�������$��2��5w   '   5x      &   System.Windows.Controls.DataGrid  5|      $��1��6      AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5�         SelectionMode$ Single=�5�         CanUserAddRows$ False��5�         CanUserDeleteRows$ False��5�      $��1q�5�      ���#DDDDDD$��0,0,0,20q�5�      
�������   RowBackground
 �����   AlternatingRowBackground �#F9FAFC   GridLinesVisibility$ 
Horizontal=�5�         HeadersVisibility$ Column=�5�         HorizontalGridLinesBrush �#E3E3E3   ColumnHeaderStyle# [ 5�      ��5}      �� *	Positions+ �� SelectedItem 5~      �� *SelectedPosition+   Columns 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ 60 6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   �� *
PositionId+   ElementStyle 5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ Center=�6F   �� 5�   "    �@��Padding$
 10,0q�6<   �� 5�   "    � ��$ SemiBold"�6?   5�      5�      4	  .System.Windows.Controls.DataGridTemplateColumn	  5�      $ Position Title��6-   $ 1* 6E     	  CellTemplate  5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�5�   ,   ��5�   0   �� *PositionTitle+y�5�   ,   �� *
IsEdditing+! �� 	Converter#! [ �� 5�   &   #��[ 5�   *   ��5�   .   �� *PositionTitle+" �� UpdateSourceTrigger$" PropertyChanged=�y�5�   *   �� *
IsEdditing+#! [  5�   #   5�      5�      	  5�      $ Description��6-   $ 2* 6B     5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $
��Wrap=�5�   -   $��Center=�5�   -   ��5�   0   �� *Description+y�5�   -   �� *
IsEdditing+#! [ �� 5�   &   #��[ 5�   -   # �� TextWrapping$
# Wrap=�5�   -   $ �� AcceptsReturn$
$ True��5�   -   $��30��5�   -   ��5�   .   �� *Description+$" PropertyChanged=�y�5�   -   �� *
IsEdditing+#! [  5�   #   5�      5�      	  5�      $ 
Created At��6-   $	 140 6A     5�      �� 5�      .R�5�   "   �� $��Center=�5�   (   $	��8,0q�5�   (   ��5�   ,   �� *	CreatedAt+% �� StringFormat$% {0:dd/MM/yyyy HH:mm}��5�   (   5�      5�      	  5�      $ Actions��6-   $	 180 6>     5�      �� 5�      .R�5�   "   � $��5q�6'   .+�5�   &   �� & �� Orientation$& 
Horizontal=�61   y�5�   1   �� *
IsEdditing+#! [ .+�5�   *   �� $��70��61   $
��Edit��5�   1   #��[ 5�   1   $��30��5�   1   ��5�   1   �� *!DataContext.EditPositionCommand+' �� RelativeSource' �� ( �� AncestorType( DataGridy�5�   1   �� * DataContext.CanManagePositions+' �� ( DataGrid#! [  ��5�   1   �� �� 5�   *   $��70��61   $��Delete��5�   1   #��[ 5�   1   $��30��5�   1   ��5�   1   �� *#!DataContext.DeletePositionCommand+' �� ( DataGridy�5�   1   �� * DataContext.CanManagePositions+' �� ( DataGrid#! [  ��5�   1   �� 5�   '   �� 5�   &   $& 
Horizontal=�61   y�5�   1   �� *
IsEdditing+#! [  .+�5�   *   �� $��70��61   $
��Save��5�   1   #��[ 5�   1   $��30��5�   1   ��5�   1   �� *DataContext.SaveCommand+' �� ( DataGrid��5�   1   �� �� 5�   *   $��70��61   $��Cancel��5�   1   #��[ 5�   1   $��30��5�   1   ��5�   1   �� *DataContext.CancelCommand+' �� ( DataGrid��5�   1   �� 5�   '   5�   #   5�      5�      5�      � 5�      $��2��6    5�      �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   .+�5�      �� y�6   �� *CanManagePositions+#! [  .+�5�      �� $��0��6   $��Add New Position��6*   #��[ 5      $
��Left=�5     $	��180��5     $��	15,5,0,15q�5     ��5     �� *
AddCommand+y�5     �� *IsAddingNew+#! [ 5     �� 5     $��1��6   ���#F9FAFC���#ADADAD$��1q�5     $ 5��5     $ 20q�5     $��15q�5     y�5	     �� *IsAddingNew+#! [  ._�5     �  5     �� 5     $
��Auto �6,   �� 5     $
��Auto �6,   �� 5     $
��Auto �6,   �� 5     $
��Auto �6,   ) � ColumnDefinitions) 5     �� 5     $	��140 �6/   �� 5     $��* �6/   .+�5     �� $��0��6$   $��0��61   $��Position Title:��5  $   $��SemiBold"�5  $   $��Center=�5  $   $��0,5,0,15q�5   $   �� 5"     $��0��6"   $��1��6/   #��[ 5$  "   $��32��5%  "   $��0,0,0,15q�5&  "   ��5#  "   �� *NewPosition.PositionTitle+$" PropertyChanged=�5&  "   �� 5(     $��1��6$   $��0��61   $��Description:��5)  $   $��SemiBold"�5*  $   $	��Top=�5+  $   $��0,5,0,15q�5,  $   �� 5.     $��1��6"   $��1��6/   #��[ 50  "   $
# Wrap=�51  "   $
$ True��52  "   $	��100��53  "   $��0,0,0,15q�54  "   ��5/  "   �� *NewPosition.Description+54  "   �� 56     $��2��6%   $��1��62   $& 
Horizontal=�57  %   $��0,10,0,0q�58  %   $��Right=�59  %   .+�5:     �� $
��Save��6%   #��[ 5<  %   $	��100��5=  %   $��0,0,10,0q�5>  %   $��20��5?  %   ��5;  %   �� *SaveNewCommand+5?  %   �� 5A     $��Cancel��6%   #��[ 5C  %   $	��100��5D  %   $��20��5E  %   ��5B  %   �� *CancelCommand+5E  %   5F     5G     5H     5I     5J     5K     5L     