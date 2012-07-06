using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace OSDConfig
{
    static class ComponentResourceManagerEx
    {
        public static void ApplyResources(this ComponentResourceManager rm, Control ctrl)
        {
            rm.ApplyResources(ctrl, ctrl.Name);
            foreach (Control subctrl in ctrl.Controls)
                ApplyResources(rm, subctrl);

            if (ctrl.ContextMenu != null)
                ApplyResources(rm, ctrl.ContextMenu);


            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                    rm.ApplyResources(col, col.Name);
            }

            if (ctrl is ToolStrip)
            {
                rm.ApplyResources((ctrl as ToolStrip).Items);
            }
        }

        public static void ApplyResources(this ComponentResourceManager rm, Menu menu)
        {
            rm.ApplyResources(menu, menu.Name);
            foreach (MenuItem submenu in menu.MenuItems)
                ApplyResources(rm, submenu);
        }

        public static void ApplyResources(this ComponentResourceManager rm, ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                rm.ApplyResources(item, item.Name);
                if (item is ToolStripMenuItem)
                    ApplyResources(rm, (item as ToolStripMenuItem).DropDownItems);
            }
        }
    }
}
