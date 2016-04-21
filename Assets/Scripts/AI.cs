﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AI : MonoBehaviour {

	public Transform target;

	private List<TileManager.Tile> tiles = new List<TileManager.Tile>();
	private TileManager manager;
	public GhostMove ghost;

	public TileManager.Tile nextTile = null;
	public TileManager.Tile targetTile;
	TileManager.Tile currentTile;


    void Awake()
	{
		manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
		tiles = manager.tiles;

		if(ghost == null)	Debug.Log ("game object ghost not found");
		if(manager == null)	Debug.Log ("game object Game Manager not found");
	}

	public void AILogic()
	{
		// get current tile
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];
		
		targetTile = GetTargetTilePerGhost();
		
		// get the next tile according to direction
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];
		
		if(nextTile.occupied || currentTile.isIntersection)
		{
			//---------------------
			// IF WE BUMP INTO WALL
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				// if ghost moves to right or left and there is wall next tile
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;
					
				}
				
				// if ghost moves to up or down and there is wall next tile
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;
					
				}
				
			}
			
			//---------------------------------------------------------------------------------------
			// IF WE ARE AT INTERSECTION
			// calculate the distance to target from each available tile and choose the shortest one
			if(currentTile.isIntersection)
			{
				
				float dist1, dist2, dist3, dist4;
				dist1 = dist2 = dist3 = dist4 = 999999f;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 		dist1 = manager.distance(currentTile.up, targetTile);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	dist2 = manager.distance(currentTile.down, targetTile);
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 	dist3 = manager.distance(currentTile.left, targetTile);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	dist4 = manager.distance(currentTile.right, targetTile);
				
				float min = Mathf.Min(dist1, dist2, dist3, dist4);
				if(min == dist1) ghost.direction = Vector3.up;
				if(min == dist2) ghost.direction = Vector3.down;
				if(min == dist3) ghost.direction = Vector3.left;
				if(min == dist4) ghost.direction = Vector3.right;
				
			}
			
		}
		
		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}

    public void RunLogic()
	{
		// get current tile
		Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
		currentTile = tiles[manager.Index ((int)currentPos.x, (int)currentPos.y)];

		// get the next tile according to direction
		if(ghost.direction.x > 0)	nextTile = tiles[manager.Index ((int)(currentPos.x+1), (int)currentPos.y)];
		if(ghost.direction.x < 0)	nextTile = tiles[manager.Index ((int)(currentPos.x-1), (int)currentPos.y)];
		if(ghost.direction.y > 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y+1))];
		if(ghost.direction.y < 0)	nextTile = tiles[manager.Index ((int)currentPos.x, (int)(currentPos.y-1))];

		//Debug.Log (ghost.direction.x + " " + ghost.direction.y);
		//Debug.Log (ghost.name + ": Next Tile (" + nextTile.x + ", " + nextTile.y + ")" );

		if(nextTile.occupied || currentTile.isIntersection)
		{
			//---------------------
			// IF WE BUMP INTO WALL
			if(nextTile.occupied && !currentTile.isIntersection)
			{
				// if ghost moves to right or left and there is wall next tile
				if(ghost.direction.x != 0)
				{
					if(currentTile.down == null)	ghost.direction = Vector3.up;
					else 							ghost.direction = Vector3.down;
					
				}
				
				// if ghost moves to up or down and there is wall next tile
				else if(ghost.direction.y != 0)
				{
					if(currentTile.left == null)	ghost.direction = Vector3.right; 
					else 							ghost.direction = Vector3.left;
					
				}
				
			}
			
			//---------------------------------------------------------------------------------------
			// IF WE ARE AT INTERSECTION
			// choose one available option at random
			if(currentTile.isIntersection)
			{
				List<TileManager.Tile> availableTiles = new List<TileManager.Tile>();
				TileManager.Tile chosenTile;
				if(currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) 			availableTiles.Add (currentTile.up);
				if(currentTile.down != null && !currentTile.down.occupied &&  !(ghost.direction.y > 0)) 	availableTiles.Add (currentTile.down);	
				if(currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) 		availableTiles.Add (currentTile.left);
				if(currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0))	availableTiles.Add (currentTile.right);

				int rand = UnityEngine.Random.Range(0, availableTiles.Count);
				chosenTile = availableTiles[rand];
				ghost.direction = Vector3.Normalize(new Vector3(chosenTile.x - currentTile.x, chosenTile.y - currentTile.y, 0));
				//Debug.Log (ghost.name + ": Chosen Tile (" + chosenTile.x + ", " + chosenTile.y + ")" );
			}
			
		}
		
		// if there is no decision to be made, designate next waypoint for the ghost
		else
		{
			ghost.direction = ghost.direction;	// setter updates the waypoint
		}
	}


    public void DefenseLogic()
    {
        // get current tile
        Vector3 currentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
        currentTile = tiles[manager.Index((int)currentPos.x, (int)currentPos.y)];

        targetTile = GetTargetTilePerGhost(); // TO-DO get target tile at area with the most pellets

        // get the next tile according to direction
        if (ghost.direction.x > 0) nextTile = tiles[manager.Index((int)(currentPos.x + 1), (int)currentPos.y)];
        if (ghost.direction.x < 0) nextTile = tiles[manager.Index((int)(currentPos.x - 1), (int)currentPos.y)];
        if (ghost.direction.y > 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y + 1))];
        if (ghost.direction.y < 0) nextTile = tiles[manager.Index((int)currentPos.x, (int)(currentPos.y - 1))];

        if (nextTile.occupied || currentTile.isIntersection)
        {
            //---------------------
            // IF WE BUMP INTO WALL
            if (nextTile.occupied && !currentTile.isIntersection)
            {
                // if ghost moves to right or left and there is wall next tile
                if (ghost.direction.x != 0)
                {
                    if (currentTile.down == null) ghost.direction = Vector3.up;
                    else ghost.direction = Vector3.down;

                }

                // if ghost moves to up or down and there is wall next tile
                else if (ghost.direction.y != 0)
                {
                    if (currentTile.left == null) ghost.direction = Vector3.right;
                    else ghost.direction = Vector3.left;

                }

            }

            //---------------------------------------------------------------------------------------
            // IF WE ARE AT INTERSECTION
            // calculate the distance to target from each available tile and choose the shortest one
            if (currentTile.isIntersection)
            {

                float dist1, dist2, dist3, dist4;
                dist1 = dist2 = dist3 = dist4 = 999999f;
                if (currentTile.up != null && !currentTile.up.occupied && !(ghost.direction.y < 0)) dist1 = manager.distance(currentTile.up, targetTile);
                if (currentTile.down != null && !currentTile.down.occupied && !(ghost.direction.y > 0)) dist2 = manager.distance(currentTile.down, targetTile);
                if (currentTile.left != null && !currentTile.left.occupied && !(ghost.direction.x > 0)) dist3 = manager.distance(currentTile.left, targetTile);
                if (currentTile.right != null && !currentTile.right.occupied && !(ghost.direction.x < 0)) dist4 = manager.distance(currentTile.right, targetTile);

                float min = Mathf.Min(dist1, dist2, dist3, dist4);
                if (min == dist1) ghost.direction = Vector3.up;
                if (min == dist2) ghost.direction = Vector3.down;
                if (min == dist3) ghost.direction = Vector3.left;
                if (min == dist4) ghost.direction = Vector3.right;

            }

        }

        // if there is no decision to be made, designate next waypoint for the ghost
        else
        {
            ghost.direction = ghost.direction;  // setter updates the waypoint
        }
    }



    TileManager.Tile GetTargetTilePerGhost()
	{
		Vector3 targetPos;
		TileManager.Tile targetTile;
		Vector3 dir;

		// get the target tile position (round it down to int so we can reach with Index() function)
		switch(name)
		{
		case "blinky":	// target = pacman
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "pinky":	// target = pacman + 4*pacman's direction (4 steps ahead of pacman)
			dir = target.GetComponent<PlayerController>().getDir();
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f) + 4*dir;

			// if pacmans going up, not 4 ahead but 4 up and 4 left is the target
			// read about it here: http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior
			// so subtract 4 from X coord from target position
			if(dir == Vector3.up)	targetPos -= new Vector3(4, 0, 0);

			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "inky":	// target = ambushVector(pacman+2 - blinky) added to pacman+2
			dir = target.GetComponent<PlayerController>().getDir();
			Vector3 blinkyPos = GameObject.Find ("blinky").transform.position;
			Vector3 ambushVector = target.position + 2*dir - blinkyPos ;
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f) + 2*dir + ambushVector;
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			break;
		case "clyde":
			targetPos = new Vector3 (target.position.x+0.499f, target.position.y+0.499f);
			targetTile = tiles[manager.Index((int)targetPos.x, (int)targetPos.y)];
			if(manager.distance(targetTile, currentTile) < 9)
				targetTile = tiles[manager.Index (0, 2)];
			break;
		default:
			targetTile = null;
			Debug.Log ("TARGET TILE NOT ASSIGNED");
			break;
		
		}
		return targetTile;
	}

    enum Distance { near, medium, far };
    enum Skill {  bad, medium, good }
    enum Rate { bad, medium, good };
    enum Time { time_short, time_med, time_long };

    public void RunFuzzyLogic()
    {
        Distance pacman_dist = FuzzyDistance(ghost.transform, GameManager.instance.pacman.transform);
        Distance ghost_dist = FuzzyDistance(ghost.transform, GameManager.instance.pacman.transform); // testing - this is supposed to be for ghosts
        Skill player_skill = PlayerSkill();

        if (!GameManager.scared) {
            if (pacman_dist == Distance.near)
            {
                // not keeping track of pellet time so removed references to pellet_short, pellet_med, pellet_long
                if (player_skill == Skill.good || player_skill == Skill.medium)
                    ghost.Chase();
            }
            else if (pacman_dist == Distance.medium)
            {
                if (player_skill == Skill.bad)
                {
                    if (ghost_dist == Distance.far)
                        ghost.Defend();
                    else if (ghost_dist == Distance.near)
                        ghost.Shy();
                }
                else if (player_skill == Skill.good || player_skill == Skill.medium)
                    ghost.Chase();
            }
            else
            {
                // pacman_dist == Distance.far
                if (player_skill == Skill.bad)
                {
                    if (ghost_dist == Distance.far || ghost_dist == Distance.medium)
                        ghost.Defend();
                    else if (ghost_dist == Distance.near)
                        ghost.Shy();
                }
                else if (player_skill == Skill.medium)
                    //if (pacman_far AND skill_med AND ghost_near) then shy_ghost_behaviour
                    ghost.Shy();

                else if (player_skill == Skill.good)
                    ghost.Chase();
            }
        } 
        
    }

    Skill PlayerSkill() {
        Time time_life = FuzzyLifeTime();       // average length of time (s) between lives
        Rate pellet_rate = FuzzyPelletRate();       // rate at which player is consuming pellets

       
        // fuzzy membership of player skill in Skill        
        if (time_life == Time.time_short || pellet_rate == Rate.bad)
            return Skill.bad;
        if (time_life == Time.time_med || pellet_rate == Rate.medium)
            return Skill.medium;
        if (time_life == Time.time_long && pellet_rate == Rate.good)
            return Skill.good;
        else
            return Skill.bad;
    }

    // fuzzy membership of rate of pellet consumption in Rate (bad, medium, good)
    // pellet consumption = pellets eaten / time elapsed
    Rate FuzzyPelletRate()
    {
        double pellet_rate = 0;
        try
        {
            // time elapsed = seconds (as of right now) since last death
            int elapsed = DateTime.Now.Subtract(GameManager.instance.lastspawnedtime).Seconds;
            pellet_rate = GameManager.instance.numpelletseaten / elapsed;

        }
        catch
        {
            pellet_rate = 0;
        }
        // fPellet consumption rate = Max (MIN (b, m1, 1, m2, g), 0) 
        double b = 1 - pellet_rate;
        double m1 = pellet_rate / 0.5;
        double m2 = (1 - pellet_rate) / 0.5;
        double g = pellet_rate;

        double[] numbers = new double[] { b, m1, 1, m2, g };
        double maxOfMin = Math.Max(numbers.Min(), 0);

        if (maxOfMin == 0 || maxOfMin == b)
            return Rate.bad;
        else if (maxOfMin == m1 || maxOfMin == 1 || maxOfMin == m2)
            return Rate.medium;
        else
            return Rate.good;

    }

    // fuzzy membership of average player lifetime in Time (short, medium, long)
    Time FuzzyLifeTime()
    {       
        double lifetime = 0;
        try
        {
            lifetime = GameManager.instance.lifetimes.Average();        // average lifetime of player
            
        }
        catch (InvalidOperationException e)
        {
            // nothing happens in the case where player hasn't died yet
            // lifetime still 0
        }

        // fuzzy average lifetime = Max (MIN (s, m1, 1, m2, l), 0) 
        double s = (GameManager.basetime * 3 - lifetime) / (GameManager.basetime * 3);
        double m1 =  lifetime / (GameManager.basetime * 3);
        double m2 = (GameManager.basetime * 6 - lifetime ) / (GameManager.basetime * 6 - GameManager.basetime * 3);
        double l = lifetime / (GameManager.basetime * 6);

        double[] numbers = new double[] { s, m1, 1, m2, l };
        double maxOfMin = Math.Max(numbers.Min(), 0);

        if (maxOfMin == 0 || maxOfMin == s)
            return Time.time_short;
        else if (maxOfMin == m1 || maxOfMin == 1 || maxOfMin == m2)
            return Time.time_med;
        else
            return Time.time_long;         
    }

    // fuzzy membership of magnitude of straight-line distance between two entities in Distance (near, medium, far)
    Distance FuzzyDistance(Transform a, Transform b)
    {
        double dist = Vector3.Distance(a.position, b.position);

        // fuzzy distance = Max (MIN (near, medium1, 1, medium2, far), 0)
        double near = (GameManager.basetime / 3 - dist) / (GameManager.basetime / 3);
        double medium1 = dist / (GameManager.basetime / 3);
        double medium2 = (GameManager.basetime * 2 / 3 - dist) / (GameManager.basetime * (2 / 3) - GameManager.basetime / 3);
        double far = (dist - GameManager.basetime / 3) / (GameManager.basetime * (2 / 3) - GameManager.basetime / 3);

        double[] numbers = new double[] { near, medium1, 1, medium2, far };
        double maxOfMin = Math.Max(numbers.Min(), 0);

        if (maxOfMin == 0 || maxOfMin == near)
            return Distance.near;
        else if (maxOfMin == medium1 || maxOfMin == 1 || maxOfMin == medium2)
            return Distance.medium;
        else
            return Distance.far;

    }


}