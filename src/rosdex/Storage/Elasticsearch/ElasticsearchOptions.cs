using System;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Rosdex.Storage.Elasticsearch
{
    public class ElasticsearchOptions
    {
        private readonly CommandOption _urlOption;
        private readonly CommandOption _userNameOption;
        private readonly CommandOption _passwordOption;
        private readonly CommandOption _indexPrefixOption;

        public ElasticsearchOptions(CommandOption urlOption, CommandOption userNameOption, CommandOption passwordOption, CommandOption indexPrefixOption)
        {
            _urlOption = urlOption;
            _userNameOption = userNameOption;
            _passwordOption = passwordOption;
            _indexPrefixOption = indexPrefixOption;
        }

        public bool TryCreateStorage(ILoggerFactory loggerFactory, out ElasticsearchStorage storage)
        {
            if (_urlOption.HasValue())
            {
                storage = new ElasticsearchStorage(
                    new Uri(_urlOption.Value()),
                    _userNameOption.Value(),
                    _passwordOption.Value(),
                    _indexPrefixOption.HasValue() ? _indexPrefixOption.Value() : ElasticsearchStorage.DefaultIndexPrefix,
                    loggerFactory.CreateLogger<ElasticsearchStorage>());
            }
            storage = null;
            return false;
        }

        public static ElasticsearchOptions Register(CommandLineApplication app)
        {
            var urlOption = app.Option("--es-url <URL>", "URL to an Elasticsearch node to write to", CommandOptionType.SingleValue);
            var userNameOption = app.Option("--es-username <USERNAME>", "Username to use to authenticate with Elasticsearch", CommandOptionType.SingleValue);
            var passwordOption = app.Option("--es-password <PASSWORD>", "Password to use to authenticate with Elasticsearch", CommandOptionType.SingleValue);
            var indexPrefixOption = app.Option("--es-index-prefix <PREFIX>", "Index prefix to apply for Elasticsearch indexes (default is 'snapshot-')", CommandOptionType.SingleValue);

            return new ElasticsearchOptions(
                urlOption,
                userNameOption,
                passwordOption,
                indexPrefixOption);
        }
    }
}
