   M S B A M L   `   `   `  ����S  OAga.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b1d788d229eb5990K5clr-namespace:Aga.Controls.Tree;assembly=Aga.ControlsAga.Controls.Tree  5       TestApp   TestApp.MainDemo   5      \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Dtree5clr-namespace:Aga.Controls.Tree;assembly=Aga.Controls    5      .��5      ]�   $� IsVirtualizing  .  $� VirtualizationMode$ 	Recycling=�6=   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $	��Top=�6.   .+�5      �� -   5        $��Small Model��5   )   �� 5	      -   5        $��	Big Model��5	   '   5
      ]� 5   
   .+�5      ]� $
��Left=�6   .+�5      �� $ 
Horizontal=�6   $	��Top=�66   .+�5      �� $��TreeList��6    $��0,5,10,0q�60   5      $   Aga.Controls.Tree.TreeListView  5      -   5        $��	_treeList��5   $   $
��Left=�65   $	��300��6K      TreeListItemNodeStyle#��[  5   
   ��5      �� 5      .R�5      �� $ 
Horizontal=�6)   .+�5   "   #   Aga.Controls.Tree.RowExpander  �� 5   "   $��Bd��5      |�. #��z��5   :    �� Padding# z��5      #��z��5      #��z��5       J� SnapsToDevicePixels . ._�5   &   �� ��60   �� *Name+6H   5    #   5!       �� Triggers 5"      �� 5#   "    �� Value$
 True��5$       �� Binding 5#   .   �� *
IsSelected+ �� RelativeSource �� 	 �� AncestorType(   "Aga.Controls.Tree.TreeListViewItem"	  .a�5%   &   �� 
 �� 
TargetName$
 Bd��6C    �� Property � �� �� Value 5&   *   C� 5'   .    C� ResourceKey# Z��6>   5)   '   �� 5*   &    � �� 5+   *   C� 5,   .   # Z��6>   5.   '   5/   #   51      53      54      ]� 56      $
��Left=�6   .+�57      �� $ 
Horizontal=�6   $	��Top=�66   .+�58      �� $��TreeView��6    $��0,5,10,0q�60   59      R� 5:      -   5        $��	_treeView��5:      ��5;      �� 5<       �� 
TargetType" Q�6    .�5=      ��  �@Q�
IsExpanded 6;   �� *
IsExpanded+ �� Mode$ TwoWay=��� 5>       �@Q�
IsSelected 6;   �� *
IsSelected+$ TwoWay=�5?      ��5B      �� 5C       �� ItemsSource 63   �� *
Children+.R�5D      �� ��6(   �� *Name+5E      5G      5H      5I      5J      5K      