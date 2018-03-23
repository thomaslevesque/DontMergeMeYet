using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    public interface IGithubSettingsProvider
    {
        GithubSettings Settings { get; }
    }
}
