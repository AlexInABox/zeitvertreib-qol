using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace ZeitvertreibQOL
{
    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        public Config()
        {
        }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool Debug { get; set; }

        [Description("The unique id of the setting.")]
        public int KeybindId { get; set; } = 202;
    }
}