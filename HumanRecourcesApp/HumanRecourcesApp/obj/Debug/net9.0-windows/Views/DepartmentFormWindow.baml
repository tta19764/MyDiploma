   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5   	   2    ,HumanResourcesApp.Views.DepartmentFormWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:HumanResourcesApp.Views     5   	     � Title$  Department Form��5   	   $	��500��5	   	   $	��600��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��20(�5      $��SemiBold"�5      $��0,0,0,20q�5      ��5      �� *	FormTitle+5      � 5   
   $��1��6    5      �� 5      $
��Auto �6    �� 5       $
��Auto �6    �� 5!      $
��Auto �6    �� 5"      $
��Auto �6    �� 5#      $
��Auto �6    �� 5$      $
��Auto �6     � ColumnDefinitions 5&      �� 5'      $	��150 �6#   �� 5(      $��* �6#   .+�5,      �� $��0��6   $��0��6%   $��Department Name:��5-      $��Center=�5.      $
��0,10q�5/      �� 50      $��0��6   $��1��6#   $��30��52      $��Center=�53      $
��0,10q�54      ��51      �� *DepartmentName+ �� UpdateSourceTrigger$ PropertyChanged=�54      �� 56      $��1��6   $��0��6%   $"��Department manager position:��57      $��Center=�58      $
��Wrap=�59      $
��0,10q�5:      �� 5;      $��1��6   $��1��6$    �� DisplayMemberPath$ PositionTitle��5>      $��30��5?      $
��0,10q�5@      ��5<      �� *	Positions+ �� SelectedItem 5=      �� *SelectedPosition+5@      �� 5C      $��2��6   $��0��6%   $��Manager:��5D      $��Center=�5E      $
��0,10q�5F      �� 5G      $��2��6   $��1��6$   $ FullName��5J      $��30��5K      $
��0,10q�5L      ��5H      �� *
Managers+ 5I      �� *SelectedManager+5L      �� 5O      $��3��6   $��0��6%   $��Created At:��5P      $��Center=�5Q      $
��0,10q�5R      �� 5S      $��3��6   $��1��6%   $��14(�5U      $��5q�5V      $��Center=�5W      $��Center=�5X      ��5T      �� *CreatedAtDisplay+5X      �� 5[      $��4��6   $��0��6%   $��Description:��5\      $	��Top=�5]      $
��0,10q�5^      �� 5_      $��4��6   $��1��6#   $	��100��5a       �� TextWrapping$
 Wrap=�5b      	 �� AcceptsReturn$
	 True��5c      "
 �� VerticalScrollBarVisibility$

 Auto=�5d      $
��0,10q�5e      ��5`      �� *Description+5e      �� 5h      $��5��6   $��0��6%   $��2��65   
���  ��$
��Wrap=�5k      $
��0,10q�5l      ��5i      �� *ValidationMessage+5l      5m      �� 5p   
   $��2��6    �� Orientation$ 
Horizontal=�5q      $��Right=�5r      $��0,20,0,0q�5s      .+�5t      �� $��Cancel��6   $	��100��5u      $��30��5v      $
��10,0q�5w      ��5x      �� *CancelCommand+��5y      ��  �� RelativeSource ��  �� AncestorType
 Window�� 5z      $
��Save��6   $	��100��5{      $��30��5|      ���#2E75B6
���������5      �� *SaveDepartmentCommand+��5�      ��  �� 
 Window5�      5�      5�      