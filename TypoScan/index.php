<?php
	header("Content-type: text/xml; charset=utf-8"); 

	$dbserver = 'mysql.reedyboy.net';
	$dbuser = 'typoscan';
	$dbpass = '256iumD3';
	$database = 'typoscan';
	
	$conn=mysql_connect($dbserver, $dbuser, $dbpass); 
	mysql_select_db($database, $conn);
		
	$query = 'SELECT articleid, title FROM articles WHERE (checkedout < DATE_SUB(NOW(), INTERVAL 2 HOUR)) AND (finished = 0) LIMIT 100';
	
	$result=mysql_query($query) or die ('Error: '.mysql_error());
	
	$xml_output  = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
	$xml_output .= "<articles>\n";

	$array = array();

	while($row = mysql_fetch_assoc($result))
	{
		$array[] = $row['articleid'];
		$therow = $row['title'];
		$xml_output .= "\t<article id='{$row['articleid']}'>";
		// Escaping illegal characters
		$therow = str_replace("&", "&amp;", $therow);
		$therow = str_replace("<", "&lt;", $therow);
		$therow = str_replace(">", "&gt;", $therow);
		$therow = str_replace("\"", "&quot;", $therow);
		$xml_output .= $therow . "</article>\n";
	}
	
	$query = 'UPDATE articles SET checkedout = NOW() WHERE articleid IN (' . implode(",", $array) . ')';
	$result=mysql_query($query) or die ('Error: '.mysql_error());
	
	$xml_output .= "</articles>";

	echo $xml_output; 
	
	mysql_close($conn);
?>