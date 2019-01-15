// <copyright>
// Copyright Southeast Christian Church

//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche.Components
{
    public interface IListViewComponent
    {
        bool IsRefreshing { get; set; }
        bool CanRefresh { get; set; }
        List<ListElement> ItemsSource { get; set; }
        object SelectedItem { get; set; }
        double Columns { get; set; }

        event EventHandler Refreshing;
        event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        event EventHandler<ItemVisibilityEventArgs> ItemAppearing;

        void Draw();
    }
}