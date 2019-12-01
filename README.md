# SuperSecretAIThingy
Private studies project for AC 2019 fall semester

Uses OpenNLP and Solr for a simple Q&A implementation

#Start up
go to your SOLR project and run
on windows
bin\solr.cmd start -c -p 8983 -s example\cloud\node1\solr
and
bin\solr.cmd start -c -p 7574 -s example\cloud\node2\solr -z localhost:9983

this will start your solr instance and enable to to search through your collections
