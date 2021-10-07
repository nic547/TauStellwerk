// <copyright file="NotifyingModelBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TauStellwerk.Client.Model
{
    public abstract class NotifyingModelBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler? PropertyChanging;

        protected void SetAndNotifyIfChanged<T>(ref T field, T value, [CallerMemberName] string caller = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(caller));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}