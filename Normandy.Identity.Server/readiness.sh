#!/bin/bash
result_code=`curl -I -m 10 -o /dev/null -s -X Get -w %{http_code} http://localhost:5000/health`
if [ "${result_code}" == "200" ]; then
   exit 0
fi
echo "service is not health......"
exit 1