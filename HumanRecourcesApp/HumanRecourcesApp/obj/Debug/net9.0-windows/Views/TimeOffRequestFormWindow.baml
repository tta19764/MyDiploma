   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5   	   E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5   	   6    0HumanResourcesApp.Views.TimeOffRequestFormWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:HumanResourcesApp.Views     5   	   ;classes'clr-namespace:HumanResourcesApp.Classes     5   	     � Title$  Time Off Request Form��5	   	   $	��650��6'   $	��700��64   ���#F4F6F9 � WindowStartupLocation$ CenterScreen=�5
       � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverter7   1HumanResourcesApp.Classes.InverseBooleanConverter  InverseBooleanConverter��%:   &	        &	        5   
   6/     5   
   6*   5      .��5      �� 
������� �� CornerRadius$ 10��6     �� Padding$ 20q�62   $��20q�6?   ���#DDD$��1q�6^   ._�5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� $��24(�5      $��SemiBold"�5      ���#3A6EA5$��0,0,0,20q�5      $
��Left=�5      ��5      �� *	FormTitle+5      �� 5"      $��1��6   $��0,0,0,20q�6&   $
��auto��68   .+�5$      �� $��Employee��6   $
��Bold"�6,   ���#555$��0,0,0,4q�6P   �� 5%       �� DisplayMemberPath$ 	FirstName��5&      $��0,0,0,12q�5(      $��8q�5)      ���#FAFAFA��5%      �� *	Employees+ �� SelectedItem 5'      �� *SelectedEmployee+{�5+      �� *CantManageTimeOffs+ �� 	Converter# [ �� 5.      $��Time Off Type��6   $
��Bold"�61   ���#555$��0,0,0,4q�6U   �� 5/      $ TimeOffTypeName��50      $��0,0,0,12q�52      $��8q�53      ���#FAFAFA��5/      �� *TimeOffTypes+ 51      �� *SelectedTimeOffType+54      �� 57      $��
Start Date��6   $
��Bold"�6.   ���#555$��0,0,0,4q�6R   (  "System.Windows.Controls.DatePicker  58      $��0,0,0,12q�59      $��6q�5:      ���#FAFAFA	   SelectedDate	 58      �� *	StartDate+5;      �� 5>      $��End Date��6   $
��Bold"�6,   ���#555$��0,0,0,4q�6P     5?      $��0,0,0,12q�5@      $��6q�5A      ���#FAFAFA	 5?      �� *	EndDate+5B      �� 5E      $��Reason for Request��6   $
��Bold"�66   ���#555$��0,0,0,4q�6Z   �� 5F      $��0,0,0,12q�5G      $��8q�5H      
 �� AcceptsReturn$

 True��5I      $��80��5J       �� TextWrapping$
 Wrap=�5K      ���#FAFAFA��5F      �� *Reason+5L      �� 5O      $��30��5P      
���  ��$��Italic$�5R      $��0,5,0,0q�5S      ��5O      �� *ValidationMessage+5S      5T      �� 5W      $��2��6    �� Orientation$ 
Horizontal=�6&   $��Right=�6?   .+�5X      �� $
��Save��6   $��5q�5[      $
��10,5q�5\      ���#3A6EA5
�������$
��Bold"�5_      $��80��5`      ��5Y      �� *SaveCommand+��5Z      ��  �� RelativeSource ��  �� AncestorType
 Window��5a      Z� �� 5c      $��Cancel��6   $��5q�5f      $
��10,5q�5g      ���#E57373
�������$
��Bold"�5j      $��80��5k      ��5d      �� *CancelCommand+��5e      ��  �� 
 Window��5l      Z� 5m      5n      5o      5p      