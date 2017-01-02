/*  GREATIS FORM DESIGNER FOR .NET
 *  Private Designer Interface Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Greatis
{
    namespace FormDesigner
    {
        #region Designer Surface

        #region TypeDescriptorFilterService
        class TypeDescriptorFilter : ITypeDescriptorFilterService
        {
            private DesignerHost host;
            internal event FilterEventHandler FilterAtt;
            internal event FilterEventHandler FilterEvnts;
            internal event FilterEventHandler FilterProps;

            public TypeDescriptorFilter(DesignerHost hst)
            {
                host = hst;
            }

            public bool FilterAttributes(IComponent component, IDictionary attributes)
            {
                bool retVal = false;
                IDesigner designer = host.Host.GetDesigner(component);
                if (designer is IDesignerFilter)
                {
                    IDesignerFilter designerFilter = (IDesignerFilter)designer;

                    designerFilter.PreFilterAttributes(attributes);
                    designerFilter.PostFilterAttributes(attributes);

                    retVal = true;
                }

                if (FilterAtt != null && !(component is DesignSurface))
                {
                    FilterEventArgs args = new FilterEventArgs();
                    args.data = attributes;
                    args.caching = true;

                    FilterAtt(component, ref args);

                    return args.caching;
                }
                return retVal;
            }

            public bool FilterEvents(IComponent component, IDictionary events)
            {
                bool retVal = false;
                IDesigner designer = host.Host.GetDesigner(component);
                if (designer is IDesignerFilter)
                {
                    IDesignerFilter designerFilter = (IDesignerFilter)designer;

                    designerFilter.PreFilterEvents(events);
                    designerFilter.PostFilterEvents(events);

                    retVal = true;
                }

                if (FilterEvnts != null && !(component is DesignSurface))
                {
                    FilterEventArgs args = new FilterEventArgs();
                    args.data = events;
                    args.caching = true;

                    FilterEvnts(component, ref args);
                    return args.caching;
                }
                return retVal;
            }

            public bool FilterProperties(IComponent component, IDictionary properties)
            {
                bool retVal = false;
                IDesigner designer = host.Host.GetDesigner(component);
                if (designer is IDesignerFilter)
                {
                    IDesignerFilter designerFilter = (IDesignerFilter)designer;

                    designerFilter.PreFilterProperties(properties);
                    designerFilter.PostFilterProperties(properties);

                    retVal = true;
                }

                if (FilterProps != null && !(component is DesignSurface))
                {
                    FilterEventArgs args = new FilterEventArgs();
                    args.data = properties;
                    args.caching = true;

                    FilterProps(component, ref args);
                    return args.caching;
                }
                return retVal;
            }


        }
        #endregion

        class HostDesignSurface : DesignSurface
        {
            public HostDesignSurface(DesignerHost host)
            {
                TypeDescriptorFilter tdfs = new TypeDescriptorFilter(host);

                tdfs.FilterAtt += new FilterEventHandler(host.FilterAtt);
                tdfs.FilterEvnts += new FilterEventHandler(host.FilterEvnts);
                tdfs.FilterProps += new FilterEventHandler(host.FilterProps);
                ServiceContainer.RemoveService(typeof(ITypeDescriptorFilterService));
                ServiceContainer.AddService(typeof(ITypeDescriptorFilterService), tdfs);
            }

            public HostDesignSurface(IServiceProvider parentProvider, DesignerHost host)
                : base(parentProvider)
            {
                ITypeDescriptorFilterService tds = (ITypeDescriptorFilterService)host.GetService(typeof(ITypeDescriptorFilterService));

                TypeDescriptorFilter tdfs = new TypeDescriptorFilter(host);

                tdfs.FilterAtt += new FilterEventHandler(host.FilterAtt);
                tdfs.FilterEvnts += new FilterEventHandler(host.FilterEvnts);
                tdfs.FilterProps += new FilterEventHandler(host.FilterProps);
                ServiceContainer.RemoveService(typeof(ITypeDescriptorFilterService));
                ServiceContainer.AddService(typeof(ITypeDescriptorFilterService), tdfs);
            }
        }
        #endregion
    }
}
