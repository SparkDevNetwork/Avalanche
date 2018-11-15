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
using System.Text;

namespace Avalanche.Models
{
    // Some notes about form response.
    // If Success is false, the form should be reshown as is with the Message
    // If there are FormElementItems the form should be redrawn with the new form elements
    // Action should be followed if set
    // Otherwise Message should be shown.

    public class FormResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string Resource { get; set; }
        public string Parameter { get; set; }
        public List<FormElementItem> FormElementItems { get; set; }
    }
}
