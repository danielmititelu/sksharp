using Newtonsoft.Json;
using Skypeulica.Utils.Models;
using System.Xml;

namespace Skypeulica.Utils
{
    internal static class ResponseParsers
    {
        internal static string GetSecTokenFromResponse(string responseXml)
        {
            //var responseXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\"><S:Header></S:Header><S:Body><wst:RequestSecurityTokenResponseCollection xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2004/04/trust\" xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2003/06/secext\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2002/12/policy\" xmlns:psf=\"http://schemas.microsoft.com/Passport/SoapServices/SOAPFault\"><wst:RequestSecurityTokenResponse><wst:TokenType>urn:passport:compact</wst:TokenType><wsp:AppliesTo xmlns:wsa=\"http://schemas.xmlsoap.org/ws/2004/03/addressing\"><wsa:EndpointReference><wsa:Address>wl.skype.com</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><wst:LifeTime><wsu:Created>2023-05-30T07:10:31Z</wsu:Created><wsu:Expires>2023-05-31T07:10:31Z</wsu:Expires></wst:LifeTime><wst:RequestedSecurityToken><wsse:BinarySecurityToken Id=\"Compact0\">t=EwAAA8hjBAAUXBDo0ce2SLKtFmOyzS4sbcCDD/EAARfJKjkoySJZ4ulL+ngen+xhVg2HEjLNZZ7IChzcjm/0nSITzp1b7fzdH9Ln5ZPukSYjRpioy3fdzZXwEKbTo7XY7vdp4QcvhBiL2i3wE6Uf7XfYmmuUPGdywdejg/cQDCd6Emf7FTjIZfdbMjhpVU8X4Gli3cHZgF+e8w6mWEYt9RBHlIDi8tP884k4/1kG8N4wl4W16ClXAWw1NvHLSsw1pKmHCOJH0K/JHUqiL6syON3DEANEv2e1tuBogFytqmS+ECJzUYQwvcO6z0NBujVvA/FALcDfGmP0BE6R9RiSWR8GqWwBaCNtNbD8cksgTPMMFhfy0IkACgNS1JjcLX8DZgAACAWdVGeLOqOH0AHHNDEVDpcDFq9tS/fYl+d5akNrSitr0Huog7sK+0MievbaiCOf7hiEJku5DxWnzQGL8DeaPvJyhMXegjEzJ4nKvwfObJRsyI4EcqYmX42gtse4anCt3Te2pYA1ZJEFYzmlRIQNIw7bn/Itfb05bQiTC9tcTI6YhEP8fFTIQiaWEtHruWiYW8hmgcvk7ITvWe8R4SNDOBLetoOe17QEOdnkoqex4PS69ryOO7VMnRRRPkI7cVvlrQFLA+gjr31hCQ56QAresUcuezg0FnSzu9ejABCJ4gHsiODkHqzrAuKIfpPJojOQmSR3hjKkvRp9v82D7135p40sL2borO8heoKQHjo997PrDxcj7a3cmSAUSaTXa/QKxlOBsBswwgIHmKNok4iyo8KjOUghBk/l3i7lpjdPn6ahbibADVNbMXWArsaOznWLxhmB6C3wPkcKNfN6dJviIfETQ82jaFFuNF6DwE7cM8TS26BsiC0bh6XLpgXLwHwu11poEUBUX2/8It92AerkivoRjh+MCIhkpC/vQXB+O0uwr2tiLk36PDGAid6wWBU3YdppNC/cJ/SoiXL2V3vPM2AgGwVzUuK2znlJrfFWStA2t35+sEeeS3iBgTYC&amp;p=</wsse:BinarySecurityToken></wst:RequestedSecurityToken><wst:RequestedTokenReference><wsse:KeyIdentifier ValueType=\"urn:passport:compact\"></wsse:KeyIdentifier><wsse:Reference URI=\"#Compact0\"></wsse:Reference></wst:RequestedTokenReference></wst:RequestSecurityTokenResponse></wst:RequestSecurityTokenResponseCollection></S:Body></S:Envelope>";
            var doc = new XmlDocument();
            doc.LoadXml(responseXml);
            var elems = doc.GetElementsByTagName("wsse:BinarySecurityToken");

            if (elems.Count == 0) { throw new Exception($"No token found, response: {responseXml}"); }

            return elems[0].InnerText;
        }

        internal static SkypeToken GetSkypeTokenFromResponse(string skypeJsonResponse)
        {
            return JsonConvert.DeserializeObject<SkypeToken>(skypeJsonResponse);
        }
    }
}
