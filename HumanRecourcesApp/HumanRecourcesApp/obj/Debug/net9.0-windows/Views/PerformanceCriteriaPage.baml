   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5      5    /HumanResourcesApp.Views.PerformanceCriteriaPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5      =controls(clr-namespace:HumanResourcesApp.Controls     5        N� Title$%  Performance Criteria Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverterC   =HumanResourcesApp.Classes.InverseBooleanToVisibilityConverter  InverseBoolToVisConverter5   /HumanResourcesApp.Classes.BoolToStringConverter  BoolToStringConverter   	TrueValue   
FalseValue4   .HumanResourcesApp.Classes.NumericOnlyConverter  NumericOnlyConverter  DataGridColumnHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value  ActionButton �� 
TargetType	 �� CornerRadius
 �� Triggers P� Property P� Value  DeleteButton �� BasedOn �� Triggers  
SaveButton  CancelButton 	 ActiveStatusStyle �� Binding �� Value��%o	  &	        &	      &	 &     &	 Q     &	 d     &	 {    &	 8    7  &	 \    7  &	 �    7  &		 �      5   
   6/     5   
   66     5   
   $	 Yes��6F   $ No��6V     5   
   6'   �� 5   
   "  62   .�5      ��  � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$
 10,8q�6(   �� 5       � ��$ 0,0,1,0q�60   �� 5       � �� �#4393E46,   5      �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 0,0,8,0q�6'   �� 5        � �� �#2E86DE6+   �� 5!       � ��
 �����6+   �� 5"       � ��$ 0q�60   �� 5#       �@�Cursor$
 Hand��6'   �� 5$       � �� 5%      �� 5&      " ��6&   .R�5'      �� #��z��6!   #��z��5(      #��z��5)      $	 4��5*      ._�5+      �� $��Center=�6/   $��Center=�6L   5,      
 5-      P� 5.       �@J�IsMouseOver$
 True��6=   . �5/   "   ��  � �� �#1D75C06?   50      P� 51       � ��$
 True��6;   . �52   "   ��  � �� �#166ABD6?   53      55      57      58      �� 5:   
   " ��6%   8   .�5;      ��  � �� �#FF52526+    5<      P� 5=       �@J�IsMouseOver$
 True��61   . �5>      ��  � �� �#E03E3E63   5?      P� 5@       � ��$
 True��6/   . �5A      ��  � �� �#C1333363   5B      5D      �� 5F   
   " ��6#   8   .�5G      ��  � �� �#27AE606+    5H      P� 5I       �@J�IsMouseOver$
 True��61   . �5J      ��  � �� �#22995463   5K      P� 5L       � ��$
 True��6/   . �5M      ��  � �� �#1E874B63   5N      5P      �� 5R   
   " ��6%   8   .�5S      ��  � �� �#7F8C8D6+    5T      P� 5U       �@J�IsMouseOver$
 True��61   . �5V      ��  � �� �#6C7A7B63   5W      P� 5X       � ��$
 True��6/   . �5Y      ��  � �� �#5E696A63   5Z      5\      �� 5^   
   " ��6*    5_      �� 5`       6   �� *
IsActive+ True6;   .a�5a      ��  � �� �#27AE6063   �� 5b       � ��$ SemiBold"�63   5c      �� 5d       6   �� *
IsActive+	 False6;   .a�5e      ��  � �� �#7F8C8D63   �� 5f       � ��$ Italic$�62   5g      5i      5j      .��5l      �  � RowDefinitions 5m   
   �� 5n      $
��Auto �6   �� 5o      $��* �6   �� 5p      $
��Auto �6   .+�5t   
   �� $��0��6   ���#2E75B6 �� Padding$ 15q�63   ._�5u      �  � ColumnDefinitions 5v      �� 5w      $��* �6'   �� 5x      $
��Auto �6'   .+�5{      �� $��Performance Criteria��6   $��22(�5|      $
��Bold"�5}      
�������$��2��5~   (   4   .HumanResourcesApp.Controls.ActionButtonControl  5�      $��1��5�      $��Right=�5�         ButtonContent$ Add Criteria��5�         ButtonPadding$
 12,8q�5�         ButtonCommand 5�      �� *AddCriteriaCommand+y�5�      �� *CanManagePerformance+ �� 	Converter# [  5�      5�      �� 5�   
   $��1��6   
�������$	 8��61   ._�5�      &   System.Windows.Controls.DataGrid  -   5          Name$ CriteriaDataGrid��5�         AutoGenerateColumns$ False��5�         
IsReadOnly$
 True��5�         CanUserAddRows$ False��5�         RowHeaderWidth$ 0Q�5�      $��1q�5�      ���#DDDDDD$��0,0,0,20q�5�      
�������   RowBackground
 �����   AlternatingRowBackground �#F9FAFC   GridLinesVisibility$ 
Horizontal=�5�          HeadersVisibility$  Column=�5�      !   HorizontalGridLinesBrush! �#E3E3E3"   ColumnHeaderStyle#" [ 5�      #   	RowHeight$# 40Q�5�      ��5�      �� *PerformanceCriteria+$ �� SelectedItem$ 5�      �� *SelectedCriteria+%   Columns% 5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�      ,	  &System.Windows.Controls.DataGridColumn& 	  Header$
& Name��6-   ' 	  Width5
  /System.Windows.Controls.DataGridLengthConverter$	' .8*
 6;   (   CellTemplate( 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6N   ��60   �� *CriteriaName+6N   5�   #   5�      5�        5�      $& Category��6-   $	' .6*
 6?   ( 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6J   ��60   �� *
Category+6J   5�   #   5�      5�        5�      $& Weight %��6-   $	' .4*
 6?   ( 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6l   ��60   �� *WeightPercentage+) �� StringFormat$) {0:F1}%��6l   5�   #   5�      5�        5�      $& Active��6-   $	' .4*
 6=   ( 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� #��[	 5�   0   $��Center=�5�   0   ��5�   0   �� *
IsActive+# [ 5�   0   5�   #   5�      5�        5�      $& 
Created On��6-   $	' .6*
 6A   ( 5�      �� 5�      .R�5�   "   � $	��4,4q�6'   .+�5�   &   �� $��Center=�6l   ��60   �� *	CreatedAt+$) {0:dd/MM/yyyy}��6l   5�   #   5�      5�        5�      $& Actions��6-   $	' .6*
 6>   ( 5�      �� 5�      .R�5�   "   �� * �� Orientation$* 
Horizontal=�6-   $	��4,4q�6F   y�5�   )   �� *" DataContext.CanManagePerformance++ �� RelativeSource+ �� , �� AncestorType",  # [  .+�5�   &   �� $��70��6-   $
��Edit��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *!DataContext.EditCriteriaCommand++ �� ",  ��5�   )   �� �� 5�   &   $��70��6-   $��Delete��5�   )   #��[ 5�   )   $��30��5�   )   ��5�   )   �� *#!DataContext.DeleteCriteriaCommand++ �� ",  ��5�   )   �� 5�   #   5�      5�      5�      5�      �� 5�   
   
�������$ 20q�6$   $	 8��61   $��15 20 15 15q�6B   $��2��6W   y�5�      �� *IsAddingOrEditing+# [  ._�5�      �� $* Vertical=�6   $	��500��60   .+�5�      �� $��18(�67   $
��Bold"�6E   $��0 0 0 10q�6W   ��6   �� *	FormTitle+6W   �� 5�      $* 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��Name:��6    $	��120��6-   $��Center=�69   �� 5�      $	��350��5�      ��5�      �� *NewCriteria.CriteriaName+- �� UpdateSourceTrigger$- PropertyChanged=�5�      5�      �� 5�      $* 
Horizontal=�6   $	��0 5q�66   .+�5�      �� $��Description:��6    $	��120��64   $	��Top=�6@   $��0,5,0,0q�6X   �� 5�      $	��350��5�      $��60��5�      . �� TextWrapping$
. Wrap=�5�      / �� AcceptsReturn$
/ True��5�      "0 �� VerticalScrollBarVisibility$
0 Auto=�5�      ��5�      �� *NewCriteria.Description+$- PropertyChanged=�5�      5�      �� 5�      $* 
Horizontal=�6   $	��0 5q�66   .+�5      �� $��	Category:��6    $	��120��61   $��Center=�6=   �� 5     1 �� 
IsEditable$
1 True��5     $	��350��5     2 �� Text2 5     �� *NewCriteria.Category+$- PropertyChanged=�.@�5     �� $��Productivity��6'   �� 5     $��Quality��6'   �� 5     $��Teamwork��6'   �� 5     $��
Leadership��6'   �� 5     $��
Innovation��6'   �� 5	     $��Communication��6'   �� 5
     $��Development��6'   5     5     �� 5     $* 
Horizontal=�6   $	��0 5q�66   .+�5     �� $��	Weight %:��6    $	��120��61   $��Center=�6=   �� 5     $	��350��5     ��5     �� *WeightPercentageText+$- PropertyChanged=�.��5      5     3 �� Path��%*  '��       7  �� 5     " ��6$    5  "   �� 5  &    62   �� + �� *Self+$
3 Text�8   	 False6�   .a�5  *   ��  � ��
 �  ��6H   �� 5  *    � ��$ 1q�6L   �� 5  *    �@�ToolTip$( "Please enter a valid numeric value��6D   5  '   5     5     .��6-   5     5     �� 5      $* 
Horizontal=�6   $	��0 5q�66   .+�5!     �� $��Active:��6    $	��120��6/   $��Center=�6;   �� 5"     $��Center=�5#     4 d� 	IsChecked4 5"     �� *NewCriteria.IsActive+$- PropertyChanged=�5#     5$     �� 5&     $* 
Horizontal=�5'     $��	0,15,20,0q�5(     $��Right=�5)     .+�5*     �� $
��Save��6   #��[ 5,     $	��100��5-     $��0,0,10,0q�5.     $��32��5/     ��5+     �� *SaveCommand+5/     �� 51     $��Cancel��6   #��[ 53     $	��100��54     $��32��55     ��52     �� *CancelCommand+55     56     57     58     59     5:     