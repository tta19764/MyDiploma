   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5   	   '    !HumanResourcesApp.Views.LoginView   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:HumanResourcesApp.Views     5   	     � Title$"  HR Management System - Login��5   	   $	��600��5	   	   $	��580��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	   ���#F0F0F0 � 	Resources 5         RoundedButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius	 �� Padding
 �� Triggers P� Property P� Value  TextBoxStyle  PasswordBoxStyle  BoolToVisibilityConverter��%S  &	        &	 R    &	 5    &	     �� 5   
   " ��6+   .�5      ��  � �� �#3498db6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 0q�60   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 4��5   !   #	 z��5   !   ._�5      �� $��Center=�6/   $��Center=�6L   5      5      5       
 5!      P� 5"       �@J�IsMouseOver$
 True��61   . �5#      ��  � �� �#2980b963   �� 5$       �@�Cursor$
 Hand��6/   5%      5'      �� 5)   
   " ��6%   .�5*      ��  � ��$ 36��6'   �� 5+       � ��$
 10,5q�6(   �� 5,       � ��$ 1q�60   �� 5-       � �� �#DDDDDD6,   �� 5.       � ��$ Center=�69   5/      �� 51   
   " G�6)   .�52      ��  � ��$ 36��6'   �� 53       � ��$
 10,5q�6(   �� 54       � ��$ 1q�60   �� 55       � �� �#DDDDDD6,   �� 56       � ��$ Center=�69   57      �� 58   
   6'   59      .��5;      �  � RowDefinitions 5<   
   �� 5=      $
��Auto �6   �� 5>      $��1* �6   �� 5?      $
��Auto �6   .+�5C   
   �� ���#3498db$��80��6&   ._�5D      �� $��Center=�6   $
��30,0q�64   .+�5E      �� $��HR Management System��6   $��24(�5F      $
��Bold"�5G      
�������5H      �� 5I      $��Sign in to your account��6   $��14(�5J      ���	#E6FFFFFF$��0,5,0,0q�5L      5M      5N      �� 5Q   
   $��1��6   
�������$ 4��5S      $��30,20,30,20q�5T      $	 30q�5U      $
��auto��5V      $��1q�5W      ���#EEEEEE._�5Z      �� $��Center=�6   .+�5\      �� $��Username��6   $��SemiBold"�5]      $��0,0,0,5q�5^      �� 5_      -   5         ��Name$ UsernameTextBox��5`      #��[ 5_      ��5a      �� *
Username+ �� UpdateSourceTrigger$ PropertyChanged=��� 5b      
���  ��$��12(�5d      $��0,3,0,15q�5e      $��15��5f      ��5b      �� *UsernameError+5f      �� 5i      $��Password��6   $��SemiBold"�5j      $��0,0,0,5q�5k      G� 5l      -   5         G�Name$ PasswordBox��5m      #��[ 5l      5n      �� 5o      
���  ��$��12(�5q      $��0,3,0,20q�5r      $��15��5s      ��5o      �� *PasswordError+5s      �� 5v      
���  ��$��SemiBold"�5x       �� TextAlignment$ Center=�5y      $��0,0,0,20q�5z      ��5v      �� *ErrorMessage+y�5{      �� *
HasError+ �� 	Converter# [ �� 5~      $��Sign In��6   #��[  5      $��45��5�      $��0,10,0,0q�5�      ��5�      �� *LoginCommand+��5�      ��  �� ElementName$ PasswordBox��5�      5�      5�      �� 5�   
   $��2��6   $0��*© 2025 Your Company. All rights reserved.��5�      ���#777777$��Center=�5�      $��0,0,0,15q�5�      5�      5�      