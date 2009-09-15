<?xml version="1.0" encoding="utf-8"?> 
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html"/>
  <xsl:template match="/">
    <h2>Contacts</h2>
    <br/>
    <table border="1" celppading="1" cellspacing="1">
      <tr>
        <th>Name</th>
        <th>Email</th>
      </tr>
      <xsl:for-each select="//contact">
        <tr>
          <td>
            <xsl:value-of select="@name"/>
          </td>
          <td>
          <xsl:value-of select="@email"/>
          </td>
        </tr>        
      </xsl:for-each>
    </table>
  </xsl:template>  
</xsl:stylesheet>
