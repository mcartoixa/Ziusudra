#!/bin/bash

if [ -f ./build/versions.env ]; then
    # xargs does not support the -d option on BSD (MacOS X)
    export $(grep -a -v -e '^#' -e '^[[:space:]]*$' .build/versions.env | tr '\n' '\0' | xargs -0 )
    grep -a -v -e '^#' -e '^[[:space:]]*$' build/versions.env | tr '\n' '\0' | xargs -0 printf "\$%s\n"
    echo
fi

case "$-" in
    *i*) _wget_interactive_options="--show-progress" ;;
      *) _wget_interactive_options= ;;
esac



if [ ! -d .tmp ]; then mkdir .tmp; fi

if [ ! -f $(pwd)/.tmp/cloc.pl ]; then
    wget -nv $_wget_interactive_options -O .tmp/cloc.pl https://github.com/AlDanial/cloc/releases/download/$_CLOC_VERSION/cloc-$_CLOC_VERSION.pl
fi



if [ -f ./.env ]; then
    # xargs does not support the -d option on BSD (MacOS X)
    export $(grep -a -v -e '^#' -e '^[[:space:]]*$' .env | tr '\n' '\0' | xargs -0 )
    grep -a -v -e '^#' -e '^[[:space:]]*$' .env | tr '\n' '\0' | xargs -0 printf "\$%s\n"
    echo
fi
