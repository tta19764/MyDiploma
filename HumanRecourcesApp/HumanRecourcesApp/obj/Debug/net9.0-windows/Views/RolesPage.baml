   M S B A M L   `   `   `  ����L  HHumanResourcesApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:HumanResourcesApp.ViewsHumanResourcesApp.Views  5      G(clr-namespace:HumanResourcesApp.ControlsHumanResourcesApp.Controls  5      E'clr-namespace:HumanResourcesApp.ClassesHumanResourcesApp.Classes  5      '    !HumanResourcesApp.Views.RolesPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:HumanResourcesApp.Views     5      =controls(clr-namespace:HumanResourcesApp.Controls     5      ;classes'clr-namespace:HumanResourcesApp.Classes     5        N� Title$  Role Management��5       N� 
Background �#F5F7FA � 	Resources 5      <   6HumanResourcesApp.Classes.BooleanToVisibilityConverter   BoolToVisConverter  ActionButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers	 P� Property
 P� Value  
EditButton �� BasedOn �� Triggers  DeleteButton��%�  &	        &	      &	 <    7  &	 `    7    5   
   6/   �� 5   
   " ��6%   .�5      ��  � ��$
 12,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��$ 30��6'   �� 5       � ��$ 75��6)   �� 5       � �� �#2E86DE6+   �� 5       � ��
 �����6+   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$ 0q�60   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5      #��z��5       $ 4��5!      ._�5"      �� $��Center=�6/   $��Center=�6L   5#       5$      P� 5%      	 �@J�IsMouseOver$

 True��6=   . �5&   "   ��  � �� �#1D75C06?   5'      P� 5(      	 � ��$

 True��6;   . �5)   "   ��  � �� �#166ABD6?   5*      5,      5.      5/      �� 51   
   " ��6#   8   .�52      ��  � �� �#4CAF506+    53      P� 54      	 �@J�IsMouseOver$

 True��61   . �55      ��  � �� �#43A04763   56      P� 57      	 � ��$

 True��6/   . �58      ��  � �� �#388E3C63   59      5;      �� 5=   
   " ��6%   8   .�5>      ��  � �� �#FF52526+    5?      P� 5@      	 �@J�IsMouseOver$

 True��61   . �5A      ��  � �� �#E03E3E63   5B      P� 5C      	 � ��$

 True��6/   . �5D      ��  � �� �#C1333363   5E      5G      5H      .��5J      �  � RowDefinitions 5K   
   �� 5L      $
��Auto �6   �� 5M      $��* �6   �� 5N      $
��Auto �6   .+�5Q   
   �� ���#2E75B6 �� Padding$ 15q�6&   $��0��63   ._�5R      �  � ColumnDefinitions 5S      �� 5T      $��* �6'   �� 5U      $
��Auto �6'   .+�5X      �� $��Roles��6   $��22(�5Y      $
��Bold"�5Z      
�������$��2��5[   (   4   .HumanResourcesApp.Controls.ActionButtonControl  5]      $��1��5^      $��Right=�5_         ButtonContent$ Add Role��5`         ButtonPadding$
 12,8q�5c         ButtonCommand 5a      �� *CreateCommand+y�5b      �� *CanManageRoles+ �� 	Converter# [  5c      5d      5e      �� 5g   
   $��1��6   .��5h      �� ���#F7F7F7.+�5i      �� ��6   �� *Roles+��5j      �� 5k      .R�5l      �� ���#DDD$��1q�5m   %   $ 8��5n   %   $��
15,15,15,0q�5o   %   $ 16q�5p   %   
�������._�5r   "   �  5s   &   �� 5t   *   $
��Auto �68   �� 5u   *   $
��Auto �68   �� 5v   *   $
��Auto �68    5x   &   �� 5y   *   $��* �6;   �� 5z   *   $
��Auto �6;   .+�5~   &   �� $��0��61   $��0��6>    �� Orientation$ 
Horizontal=�6N   .+�5   *   �� $��18(�5�   3   $
��Bold"�5�   3   ���#333��5   4   �� *
RoleName+5�   3   5�   '   �� 5�   &   $��1��61   $��0��6>   $��0,8,0,0q�6N   .+�5�   *   �� ���#666$
��Wrap=�5�   3   ��5�   4   �� *Description+5�   3   5�   '   �� 5�   &   $��2��61   $��0��6>   $ 
Horizontal=�6N   $��0,10,0,0q�6g   .+�5�   *   �� �� 5�   .   $�	Created: ��62   $��SemiBold"�6C    �� 5�   .   ���#444�62   �� *	CreatedAt+ �� StringFormat$ {0:MM/dd/yyyy}��6l   5�   +   �� 5�   *   $	�� | ��64   $	��6,0q�6?   �� 5�   *   �� 5�   .   $�Permissions: ��62    �� 5�   .   $��SemiBold"�6S   �62   �� *PermissionCount+6S   5�   +   5�   '   �� 5�   &   $��3��61   $��1��6B   $ Vertical=�5�   0   $��Right=�5�   0   $��Center=�5�   0   $��10,0,0,0q�5�   0   .+�5�   *   �� $
��Edit��61   #��[ 5�   1   ��5�   1   �� *DataContext.EditRoleCommand+ �� RelativeSource ��  �� AncestorType Pagey�5�   1   �� *DataContext.CanManageRoles+ ��  Page# [  ��5�   1   �� �� 5�   *   $��Delete��61   #��[ 5�   1   ��5�   1   �� *DataContext.DeleteRoleCommand+ ��  Pagey�5�   1   �� *DataContext.CanManageRoles+ ��  Page# [  ��5�   1   �� 5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      