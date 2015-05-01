/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.request.clashgame;

import java.io.DataInputStream;
import java.io.IOException;

import core.GameServer;
import db.PlayerDAO;
import db.clashgame.DefenseConfigDAO;
import net.request.GameRequest;
import net.response.clashgame.ResponseClashEndBattle;
import util.DataReader;
import java.util.Date;
import model.Player;
import model.clashgame.DefenseConfig;

import db.clashgame.BattleDAO;
import model.clashgame.Battle;

/**
 *
 * @author lev
 */
public class RequestClashEndBattle extends GameRequest {
    
    Battle.Outcome outcome;
    
    @Override
    public void parse(DataInputStream dataInput) throws IOException {
        int value = DataReader.readInt(dataInput);
        
        if (value == 0) {
            outcome = Battle.Outcome.WIN;
        } else if (value == 1) {
            outcome = Battle.Outcome.LOSE;
        } else {
            outcome = Battle.Outcome.DRAW;
        }
    }

    @Override
    public void process() throws Exception {
        Player p = client.getPlayer();

        //record state in db
        Battle battle = BattleDAO.findActiveByPlayer(p.getID());
        battle.outcome = outcome;
        battle.timeEnded = new Date();
        BattleDAO.save(battle);

        DefenseConfig df = DefenseConfigDAO.findByDefenseConfigId(battle.defenseConfigId);
        Player defender = PlayerDAO.getPlayer(df.playerId);

        int attackerCredits = p.getCredits();
        int defenderCredits = defender.getCredits();

        switch (outcome){
            case WIN:
                attackerCredits += 100;
            case LOSE:
                attackerCredits -= 100;
                defenderCredits += 100;
            case DRAW:
                break;
        }

        PlayerDAO.updateCredits(p.getID(), attackerCredits);
        PlayerDAO.updateCredits(defender.getID(), defenderCredits);

        ResponseClashEndBattle response = new ResponseClashEndBattle();
        response.setCredits(attackerCredits);
        client.add(response);
    }
    
}
