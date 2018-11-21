using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Application
{
    public interface ISearchParam
    {
        MySearchUtil ToSearchUtil();
    }
}
