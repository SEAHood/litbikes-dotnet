﻿using LitBikes.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using LitBikes.Model;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Exceptions;

namespace LitBikes.Game.Controller
{
    public class ClientEventHandler
    {
        // CLIENT EVENTS
        /*public void ClientJoiningGame(Guid playerId, ClientGameJoinDto gameJoinDto)
        {
            if (!gameJoinDto.IsValid())
                throw new InvalidPayloadException("Invalid name"); // TODO Refactor - error from IsValid()

            //int pid = sessionPlayers.get(client.getSessionId());
            //String name = gameJoinDto.name;
            var player = _game.PlayerJoin(pid, name, true);

            GameSettingsDto gameSettings = new GameSettingsDto();
            gameSettings.gameTickMs = game.getGameTickMs();

            GameJoinDto dto = new GameJoinDto();
            dto.player = player.getDto();
            dto.scores = game.getScores();

            balanceBots();

            client.sendEvent(C_JOINED_GAME, dto);
        }*/

        public void ClientHello()
        {
            /*int pid = pidGen++;
            sessionPlayers.put(client.getSessionId(), pid);

            GameSettingsDto gameSettings = new GameSettingsDto();
            gameSettings.gameTickMs = game.getGameTickMs();

            HelloDto dto = new HelloDto();
            dto.gameSettings = gameSettings;
            dto.world = game.getWorldDto();

            client.sendEvent(C_HELLO, dto);*/
        }

        public void ClientDisconnectEvent()
        {
            /*try
            {
                Integer pid = sessionPlayers.get(client.getSessionId());
                if (pid == null)
                    return;//throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");
                game.dropPlayer(pid);
            }
            catch (Exception e)
            {

            }
            balanceBots();*/
        }

        /*public void ClientChatMessageEvent(Player player, string message)
        {
            var Colour = player.GetBike().GetColour();
            var sourceColour = $"rgba({Colour.R:X2},{Colour.G:X2},{Colour.B:X2},%A%)";
            var dto = new ChatMessageDto(player.GetName(), sourceColour, message, false);

            // TODO BROADCAST THAT SHIT

            //BroadcastData("chat-message", dto);
        }*/


        /*public void ClientUpdateEvent(ClientUpdateDto updateDto)
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                return;//throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            if (game.HandleClientUpdate(updateDto))
            {
                broadcastWorldUpdate();
            }#1#
        }*/


        /*public void ClientRequestRespawnEvent()
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            Player player = game.getPlayer(pid);
            LOG.info("Respawn request from " + player.getName());
            game.requestRespawn(player);#1#
        }*/


        /*public void ClientRequestUsePowerUpEvent()
        {
            /*Integer pid = sessionPlayers.get(client.getSessionId());
            if (pid == null)
                throw new Exception("sessionPlayers value was null.. this really shouldn't have happened");

            Player player = game.getPlayer(pid);
            if (player.getCurrentPowerUpType() == null)
                return; // player doesn't have a powerup

            LOG.info("PowerUp used by " + player.getName());
            game.requestUsePowerUp(player);
            broadcastWorldUpdate();#1#
        }*/


        public void ClientHelloEvent()
        {
            /*LOG.info("Received hello");
            clientHello(client);*/
        }

        public void ClientRequestWorldEvent()
        {
            //SendWorldUpdate(client);
        }

        public void ClientKeepAliveEvent()
        {
            // todo time out client after 2 missed keep alives or something?
            //client.sendEvent("keep-alive-ack");
        }

        /*public void ClientRequestGameJoinEvent(ClientGameJoinDto gameJoinDto)
        {
            /*LOG.info("Received game join request event");
            clientJoiningGame(client, gameJoinDto);

            // TODO Make sure client isn't trying to rejoin - i.e. if they already have a name

            String newPlayerMessage = gameJoinDto.name + " joined!";
            ChatMessageDto dto = new ChatMessageDto(null, null, newPlayerMessage, true);
            broadcastData("chat-message", dto);#1#
        }*/

        // END CLIENT EVENTS

    }
}
